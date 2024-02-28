using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OnlineRecruitment_Main.CustomModels;
using OnlineRecruitment_Main.Models;
using vrecruit.DataBase.Comman;
using vrecruit.DataBase.EntityDataModel;
using vrecruit.DataBase.ViewModel;
using log4net;
using System.Threading;
using System.Text.RegularExpressions;

namespace OnlineRecruitment_Main.Controllers
{
    ///[Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        vRecruitEntities db = new vRecruitEntities();
        ApplicationDbContext context = new ApplicationDbContext();
        //Declare an instance for log4net
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Session["CurrentUserId"] != null)
            {
                return RedirectToAction("Dashboard", "Master");
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
         
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //if (!ModelState.IsValid)
            //{
            //    return Json(new { Error = true, Message = "Invalid login attempt." });
            //}
            try
            {
                if (Session["CurrentUserId"] != null)
                {
                    var curntUserId = Session["CurrentUserId"].ToString();
                    var Roles = Session["CurrentRole"].ToString();
                    var loginuser = db.AspNetUsers.Where(x => x.Id == curntUserId).Select(x => x.UserName).SingleOrDefault();
                    return Json(new { Error = true, Message = "Login", Data = loginuser, Role = Roles });
                }
                else
                {
                    var IsActive = db.AspNetUsers.Where(x => x.UserName == model.Email).Select(x => x.IsActive).FirstOrDefault();
                    var IsEmailActive = db.AspNetUsers.Where(x => x.Email == model.Email).Select(x => x.IsActive).FirstOrDefault();
                    if (IsActive == true || IsEmailActive == true)
                    {
                        Microsoft.AspNet.Identity.Owin.SignInStatus result;
                        if (IsEmailActive == true)
                        {
                            var user = UserManager.FindByEmail(model.Email);
                            result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
                        }
                        else
                        {
                            result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                        }
                        switch (result)
                        {
                            case SignInStatus.Success:
                                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                                ApplicationUser user;
                                if (IsEmailActive == true)
                                {
                                    user = UserManager.FindByEmail(model.Email);
                                }
                                else { 
                                     user = UserManager.FindByName(model.Email);
                                    }
                                var roles = await UserManager.GetRolesAsync(user.Id);
                                //var currentCopamnyId = db.Companies.Where(x => x.UserID == user.Id).Select(x => x.ID).FirstOrDefault();
                                //var currentCopamnyName = db.Companies.Where(x => x.UserID == user.Id).Select(x => x.CompanyName).FirstOrDefault();
                                AspNetUser uData = db.AspNetUsers.Where(x => x.Id == user.Id).SingleOrDefault();
                                var currentCopamnyId = uData.CompanyId;
                                var currentCopamnyName = db.Companies.Where(x => x.ID == uData.CompanyId).Select(x => x.CompanyName).FirstOrDefault();
                                var roleId = user.Roles.Select(x => x.RoleId).FirstOrDefault();
                                Session["CurrentClientId"] = uData.ClientId;
                                List<AccessPermission> Accesslist = db.AccessPermissions.Where(x => x.RoleId == roleId).OrderBy(e => e.Entity.EntityName).ToList();
                                List<MenuModels> MenuList = new List<MenuModels>();
                                foreach (var item in Accesslist)
                                {
                                    MenuModels Menu = new MenuModels();
                                    Menu.EntityId = Convert.ToInt32(item.EntityId);
                                    Menu.Entityname = item.Entity.EntityName;
                                    Menu.IsView = Convert.ToBoolean(item.IsView);
                                    MenuList.Add(Menu);
                                }
                                Session["AccesspermissionList"] = MenuList;
                                Session["currentCompanyId"] = currentCopamnyId;
                                Session["currentCompanyName"] = currentCopamnyName;
                                Session["CurrentRole"] = roles[0];
                                Session["CurrentUserId"] = user.Id;
                                Session["CurrentRoleId"] = roleId;
                                if (roles != null)
                                {
                                    if (roles[0] == "CLIENT" || roles[0] == "COMPANY USER")
                                    {
                                        var currentCmpId = db.AspNetUsers.Where(x => x.Id == user.Id).Select(x => x.CompanyId).FirstOrDefault();
                                        var cmpName = db.Companies.Where(x => x.ID == currentCmpId).Select(x => x.CompanyName).FirstOrDefault();
                                        Session["currentCompanyId"] = currentCmpId;
                                        Session["currentCompanyName"] = cmpName;
                                    }

                                    bool isClientOrCandidatePopup = false;
                                    bool isCandidateMissing = false;
                                    bool isClientMissing = false;
                                    string message = "";

                                    if (roles[0] == "SUPER ADMIN" || roles[0] == "ADMIN")
                                    {
                                        var client = db.Clients.Where(x => x.CompanyId == currentCopamnyId && x.ClientEmail == null).ToList();
                                        var candidate = db.Candidates.Where(x => x.CompanyId == currentCopamnyId && x.CandidateEmail == null).ToList();

                                        if (client.Count > 0)
                                        {
                                            isClientMissing = true;
                                            
                                            if (candidate.Count > 0)
                                            {
                                                isCandidateMissing = true;
                                            }
                                        }
                                        else
                                        {
                                            if (candidate.Count > 0)
                                            {
                                                isCandidateMissing = true;
                                            }
                                        }
                                    }

                                    if (roles[0] == "TALENT ACQUISITION" || roles[0] == "USER")
                                    {
                                        var candidate = db.Candidates.Where(x => x.CreatedBy == new Guid(user.Id) && x.CandidateEmail == null).ToList();

                                        if (candidate.Count > 0)
                                        {
                                            isCandidateMissing = true;
                                        }
                                    }

                                    if(isCandidateMissing && isClientMissing)
                                    {
                                        isClientOrCandidatePopup = true;
                                        message = "There are some incomplete client and candidate data, Please go to client/Candidate tab and complete";
                                    }
                                    else if(!isClientMissing && isCandidateMissing)
                                    {
                                        isClientOrCandidatePopup = true;
                                        message = "There are some incomplete candidate data, Please go to candidate tab and complete";
                                    }

                                    

                                    return Json(new { Error = false, Role = roles[0], IsClientOrCandidatePopup = isClientOrCandidatePopup, Message = message });
                                }
                                return RedirectToAction("Login", "Account");
                            //return RedirectToLocal(returnUrl);
                            case SignInStatus.LockedOut:
                                return View("Lockout");
                            case SignInStatus.RequiresVerification:
                                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                            case SignInStatus.Failure:
                            default:
                                return Json(new { Error = true, Message = "Invalid login attempt." });
                        }
                    }
                    else
                    {
                        return Json(new { Error = true, Message = "Unauthorized: Access is denied." });
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error("Start log ERROR...");
                Log.Error(ex.Message);
                Thread.Sleep(TimeSpan.FromSeconds(secs));
            }
            return null;
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }//
        #region profile
        public async Task<ActionResult> Profile()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var Uid = User.Identity.GetUserId();
            var roles = await UserManager.GetRolesAsync(Uid);
            AspNetUser UserModel = db.AspNetUsers.Where(x => x.Id == Uid).SingleOrDefault();
            UserViewModel VM = new UserViewModel();
            VM.Id = UserModel.Id;
            VM.Email = UserModel.Email;
            VM.PhoneNumber = UserModel.PhoneNumber;
            VM.UserName = Regex.Replace(UserModel.UserName, @"\s+", "");
            VM.ClientId = Convert.ToInt32(UserModel.ClientId);
            if (VM.ClientId != 0)
            {
                Session["CurrentClientId"] = UserModel.ClientId;
                Client client = db.Clients.Where(x => x.ClientId == VM.ClientId).SingleOrDefault();
                VM.ClientAddress = client.ClientAddress;
                VM.ClientGST = client.ClientGST;
                VM.ClientMobile = client.ClientMobile;
                VM.ClientName = client.ClientName;
                VM.ClientPhone = client.ClientPhone;
                VM.ClientWWW = client.ClientWWW;
            }
            if (roles[0] == "ADMIN")
            {
                Company company = db.Companies.Where(x => x.UserID == Uid).SingleOrDefault();
                VM.Company = company;
            }
            return View(VM);
        }
        #endregion
        #region Pssword
        [HttpPost]
        public ActionResult ResetUserPassword(string userId, string newPassword, bool? reset)
        {
            try
            {
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                userManager.RemovePassword(userId);
                var result = userManager.AddPassword(userId, newPassword);
                if (result.Succeeded)
                {
                    if (reset == true)
                    {
                        return Json(new { error = false, message = "Reset" });
                    }
                    else
                    {
                        return Json(new { error = false, message = "success" });
                    }
                }
                else
                {
                    return Json(new { error = true, message = result });
                }
            }
            catch (Exception ex)
            {
                Log.Error("Start log ERROR...");
                Log.Error(ex.Message);
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                return Json(new { Error = true, Data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult ChangePassword(ResetPasswordViewModel model)
        {
            try
            {
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                string uid = User.Identity.GetUserId();
                var result = userManager.ChangePassword(uid, model.CurrentPassword, model.Password);
                if (result.Succeeded)
                {
                    return Json(new { error = false, message = "success" });
                }
                else
                {
                    return Json(new { error = true, message = result });
                }
            }
            catch (Exception ex)
            {
                Log.Error("Start log ERROR...");
                Log.Error(ex.Message);
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                return Json(new { Error = true, Data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region CreateClient
        [HttpPost]
        public async Task<ActionResult> SaveClientToUserTbl(string ClientEmail, string Password, string ClientId, string username)
        {
            if (ModelState.IsValid)
            {
                if (ClientId != "")
                {
                    int cid = Convert.ToInt32(ClientId);
                    AspNetUser Udata = db.AspNetUsers.Where(x => x.ClientId == cid).SingleOrDefault();
                    Udata.Email = ClientEmail;
                    if (username != "")
                    {
                        Udata.UserName = Regex.Replace(username, @"\s+", "");
                    }
                    Udata.IsActive = true;
                    db.SaveChanges();
                    return Json(new { error = false, message = "success", Data = Udata.Id });
                }
                else
                {
                    var user = new ApplicationUser { UserName = username, Email = ClientEmail };
                    var result = await UserManager.CreateAsync(user, Password);
                    if (result.Succeeded)
                    {
                        AspNetUser aspNetUser = db.AspNetUsers.Find(user.Id);
                        aspNetUser.UserName = Regex.Replace(username, @"\s+", "");
                        aspNetUser.Email = ClientEmail;
                        aspNetUser.IsActive = true;
                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                        if (!roleManager.RoleExists("CLIENT"))
                        {
                            var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                            role.Name = "CLIENT";
                            roleManager.Create(role);
                        }
                        UserManager.AddToRole(user.Id, "CLIENT");
                        db.SaveChanges();
                        return Json(new { error = false, message = "success", Data = user.Id });
                    }
                    AddErrors(result);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Message = "IsValid Model" }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        //
        #region Roles
        [HttpGet]
        [AllowAnonymous]
        public ActionResult RolesList()
        {
            List<RolesViewModel> model = new List<RolesViewModel>();
            try
            {
                List<AspNetRole> ListAspNetRole = new List<AspNetRole>();
                if (Session["CurrentRole"].ToString() == "SUPER ADMIN")
                {
                    ListAspNetRole = db.AspNetRoles.ToList();

                }
                else
                {
                    var currentuser = Session["CurrentUserId"].ToString();
                    ListAspNetRole = db.AspNetRoles.Where(x => x.LastUpdatedBy == currentuser).ToList();
                }

                if (ListAspNetRole != null)
                {
                    foreach (var item in ListAspNetRole)
                    {
                        RolesViewModel VM = new RolesViewModel();
                        VM.Id = item.Id;
                        VM.Name = item.Name;
                        VM.LastUpdatedBy = db.AspNetUsers.Where(x => x.Id == item.LastUpdatedBy).Select(x => x.UserName).FirstOrDefault();
                        model.Add(VM);
                    }
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Error = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Start log ERROR...");
                Log.Error(ex.Message);
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                return Json(new { Error = true, Data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CreateRoles(AspNetRole aspNetRole)
        {
            AspNetRole roleManager = db.AspNetRoles.Where(x => x.Name == aspNetRole.Name).FirstOrDefault();
            if (roleManager == null)
            {
                try
                {
                    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                    aspNetRole.Id = role.Id;
                    aspNetRole.Name = aspNetRole.Name.ToUpper();
                    aspNetRole.LastUpdatedBy = Session["CurrentUserId"].ToString();
                    db.AspNetRoles.Add(aspNetRole);
                    var Entitylist = db.Entities.ToList();
                    List<AccessPermission> dbentitylist = new List<AccessPermission>();
                    foreach (var item in Entitylist)
                    {
                        AccessPermission Acc = new AccessPermission();
                        Acc.EntityId = item.Id;
                        Acc.RoleId = role.Id;
                        Acc.IsAdd = false;
                        Acc.IsDelete = false;
                        Acc.IsEdit = false;
                        Acc.IsExport = false;
                        Acc.IsPrint = false;
                        Acc.IsView = false;
                        dbentitylist.Add(Acc);
                    }
                    db.AccessPermissions.AddRange(dbentitylist);
                    db.SaveChanges();
                    return Json(new { Message = "Inserted" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Error = true, Message = "Role Already Exists...!!!" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult DeleteRoles(string key)
        {
            var Roles = db.AspNetRoles.Where(x => x.Id == key).SingleOrDefault();
            if (Roles != null)
            {
                try
                {
                    List<AccessPermission> AccModel = db.AccessPermissions.Where(x => x.RoleId == key).ToList();
                    db.AccessPermissions.RemoveRange(AccModel);
                    db.AspNetRoles.Remove(Roles);
                    db.SaveChanges();
                    return Json(new { Message = "Delete" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(new { Error = true, Message = "Role Already Exists...!!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetRolById(string Key)
        {
            AspNetRole AspNetRoledata = new AspNetRole();
            try
            {
                AspNetRoledata = db.AspNetRoles.Find(Key);
                if (AspNetRoledata != null)
                {
                    return Json(new { Message = "Edit", Id = AspNetRoledata.Id, Name = AspNetRoledata.Name }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Error = true, Message = "Roles does not exist...!!!" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                Log.Error("Start log ERROR...");
                Log.Error(ex.Message);
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateRole(AspNetRole aspNetRole)
        {
            var Roles = db.AspNetRoles.Where(x => x.Id == aspNetRole.Id).SingleOrDefault();
            if (Roles != null)
            {
                try
                {
                    Roles.Name = aspNetRole.Name.ToUpper();
                    Roles.LastUpdatedBy = Session["CurrentUserId"].ToString();
                    db.SaveChanges();
                    return Json(new { Message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Error = true, Message = "Roles does not exist...!!!" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region PermissionAssignment
        public ActionResult GetAccessPermissionByRole(string Key)
        {
            if (Key != "")
            {
                var data = db.AccessPermissions.Where(x => x.RoleId == Key).ToList();
                List<AccessPermissionVM> AccessList = new List<AccessPermissionVM>();

                foreach (var item in data)
                {
                    AccessPermissionVM Access = new AccessPermissionVM();
                    Access.Id = item.Id;
                    Access.EntityId = Convert.ToInt32(item.EntityId);
                    Access.Entityname = item.Entity.EntityName;
                    Access.IsAdd = Convert.ToBoolean(item.IsAdd);
                    Access.IsDelete = Convert.ToBoolean(item.IsDelete);
                    Access.IsEdit = Convert.ToBoolean(item.IsEdit);
                    Access.IsExport = Convert.ToBoolean(item.IsExport);
                    Access.IsPrint = Convert.ToBoolean(item.IsPrint);
                    Access.IsView = Convert.ToBoolean(item.IsView);
                    AccessList.Add(Access);
                }
                return Json(new { Error = false, Message = "AccessList", Data = AccessList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Error = true, Message = "Entitys does not exist For Role...!!!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateAccessPermisionByRole(int? AccesssId, string Flag, bool value)
        {

            if (AccesssId != null)
            {
                try
                {
                    AccessPermission data = db.AccessPermissions.Find(AccesssId);
                    if (Flag == "IsAdd")
                    {
                        data.IsAdd = value;
                    }
                    if (Flag == "IsView")
                    {
                        data.IsView = value;
                    }
                    if (Flag == "IsEdit")
                    {
                        data.IsEdit = value;
                    }
                    if (Flag == "IsDelete")
                    {
                        data.IsDelete = value;
                    }
                    if (Flag == "IsPrint")
                    {
                        data.IsPrint = value;
                    }
                    if (Flag == "IsExport")
                    {
                        data.IsExport = value;
                    }
                    db.SaveChanges();
                    return Json(new { Error = false, Message = "AccessS Assign Successfully...!!!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json(new { Error = true, Data = ex.Message }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { Error = true, Message = "Entitys does not exist For Role...!!!" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        //
        #region CreateAdmin
        //
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetuserData(string Key)
        {
            if (Key != null)
            {
                var userdata = db.AspNetUsers.Where(aspNetUser => aspNetUser.Id == Key).FirstOrDefault();
                UserViewModel user = new UserViewModel();
                user.Id = userdata.Id;
                user.UserName = Regex.Replace(userdata.UserName, @"\s+", "");
                user.Email = userdata.Email;
                user.CompanyId = userdata.CompanyId > 0 ? (int)userdata.CompanyId :  userdata.Companies.Select(x => x.ID).FirstOrDefault();
                user.PhoneNumber = userdata.PhoneNumber;
                user.RoleName = userdata.AspNetRoles.Select(x => x.Name).FirstOrDefault();
                return Json(new { Message = "Edit", Data = user }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Error = true }, JsonRequestBehavior.AllowGet);
        }
        //
        [HttpGet]
        [AllowAnonymous]
        public ActionResult DeleteUser(string Key)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(Key);
            if (aspNetUser == null)
            {
                return Json(new { Error = true }, JsonRequestBehavior.AllowGet);
            }

            var data = db.Companies.Where(x => x.UserID == Key).SingleOrDefault();
            if (data != null)
            {
                data.UserID = null;
                db.SaveChanges();
            }
            db.AspNetUsers.Remove(aspNetUser);
            db.SaveChanges();
            return Json(new { Message = "Deleted" }, JsonRequestBehavior.AllowGet);
        }
        //
        //[HttpPost]
        //[AllowAnonymous]
        public async Task<ActionResult> UpdateUser(UserViewModel model)
        {
            if (model != null)
            {
                AspNetUser aspNetUser = db.AspNetUsers.Find(model.Id);
                if (aspNetUser != null)
                {
                    aspNetUser.UserName = Regex.Replace(model.UserName, @"\s+", ""); 
                    aspNetUser.Email = model.Email;
                    aspNetUser.PhoneNumber = model.PhoneNumber;
                    aspNetUser.CompanyId = model.CompanyId;
                    aspNetUser.LastUpdateDate = DateTime.Now.Date;
                    if (model.RoleName != null)
                    {
                        var user = new ApplicationUser { UserName = aspNetUser.UserName, Email = aspNetUser.Email };
                        var result = await UserManager.CreateAsync(user);
                        var roles = UserManager.GetRoles(model.Id).FirstOrDefault();
                        if (roles == null)
                        {
                            UserManager.AddToRole(model.Id, model.RoleName);
                        }
                        else
                        {
                            UserManager.RemoveFromRole(model.Id, roles);
                            //then add new role 
                            UserManager.AddToRole(model.Id, model.RoleName);
                        }
                    }
                    if (model.CompanyId != 0)
                    {
                        var cmpData = db.Companies.Find(model.CompanyId);
                        if (cmpData != null)
                        {
                            cmpData.UserID = model.Id;
                        }
                    }
                    try
                    {
                        db.SaveChanges();
                        return Json(new { Message = "Updated" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Start log ERROR...");
                        Log.Error(ex.Message);
                        Thread.Sleep(TimeSpan.FromSeconds(secs));
                        return Json(new { Error = true, Data = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Error = true }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { Error = true }, JsonRequestBehavior.AllowGet);
            }
        }
        //
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> CreateAdmin()
        {
            List<UserViewModel> lstuser = new List<UserViewModel>();
            var createdby = Session["CurrentUserId"].ToString();
            var userlist = db.AspNetUsers.Where(x => x.CreatedBy == createdby && x.ClientId == null).ToList();
            foreach (var item in userlist)
            {
                UserViewModel user = new UserViewModel();
                user.Id = item.Id;
                user.UserName = Regex.Replace(item.UserName, @"\s+", "");
                user.Email = item.Email;
                user.CompanyName = item.Companies.Select(x => x.CompanyName).FirstOrDefault();
                user.PhoneNumber = item.PhoneNumber;
                // user.CompanyAddress = item.CompanyAddress;
                user.RoleName = item.AspNetRoles.Select(x => x.Name).FirstOrDefault();
                lstuser.Add(user);
            }
            return Json(lstuser, JsonRequestBehavior.AllowGet);
        }


        // POST: /Account/CreateAdmin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAdmin(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != null)
                {
                    AspNetUser aspNetUser = db.AspNetUsers.Find(model.Id);
                    if (aspNetUser != null)
                    {
                        aspNetUser.UserName = Regex.Replace(model.UserName, @"\s+", "");
                        aspNetUser.Email = model.Email;
                        aspNetUser.PhoneNumber = model.PhoneNumber;
                        aspNetUser.CompanyId = model.CompanyId;
                        aspNetUser.LastUpdateDate = DateTime.Now.Date;
                        //aspNetUser.CompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                        if (model.RoleName != null)
                        {
                            var user = new ApplicationUser { UserName = aspNetUser.UserName, Email = aspNetUser.Email };
                            var result = await UserManager.CreateAsync(user);
                            var roles = UserManager.GetRoles(model.Id).FirstOrDefault();
                            if (roles == null)
                            {
                                UserManager.AddToRole(model.Id, model.RoleName);
                            }
                            else
                            {
                                UserManager.RemoveFromRole(model.Id, roles);
                                //then add new role 
                                UserManager.AddToRole(model.Id, model.RoleName);
                            }
                        }
                        if (model.CompanyId != 0)
                        {
                            var cmpData = db.Companies.Find(model.CompanyId);
                            if (cmpData != null)
                            {
                                cmpData.UserID = model.Id;
                            }
                        }
                        try
                        {
                            db.SaveChanges();
                            return Json(new { Message = "Updated" }, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Start log ERROR...");
                            Log.Error(ex.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(secs));
                            return Json(new { Error = true, Data = ex.Message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Error = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        AspNetUser aspNetUser = db.AspNetUsers.Find(user.Id);
                        aspNetUser.UserName = Regex.Replace(model.UserName, @"\s+", "");
                        aspNetUser.Email = model.Email;
                        aspNetUser.PhoneNumber = model.PhoneNumber;
                        aspNetUser.CompanyId = model.CompanyId > 0 ? model.CompanyId : Convert.ToInt32(Session["currentCompanyId"]);
                        aspNetUser.CreatedBy = Session["CurrentUserId"].ToString();
                        //aspNetUser.CompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                        aspNetUser.IsActive = true;
                        if (model.RoleName != null)
                        {
                            var roles = UserManager.GetRoles(user.Id).FirstOrDefault();
                            UserManager.AddToRole(user.Id, model.RoleName);
                        }
                        if (model.CompanyId != 0)
                        {
                            var cmpData = db.Companies.Find(model.CompanyId);
                            if (cmpData != null)
                            {
                                cmpData.UserID = user.Id;
                            }
                        }
                        aspNetUser.LastUpdateDate = DateTime.Now.Date;
                        db.SaveChanges();
                        return Json(new { Message = "Inserted" }, JsonRequestBehavior.AllowGet);
                    }
                    AddErrors(result);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            return View(model);
        }
        #endregion



        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                Comman comman = new Comman();
                var user = await UserManager.FindByNameAsync(model.Email);
                string link = WebConfigurationManager.AppSettings["Forgotpsd"].ToString() + user.Id;
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    // return View("ForgotPasswordConfirmation");
                    EmailViewModel EVM = new EmailViewModel();
                    EmailViewModels Emodels = new EmailViewModels();
                    Emodels.email = model.Email;
                    Emodels.to = model.Email;
                    Emodels.passwordlink = link;
                    Emodels.Smtpemail = EVM.Smtpemail;
                    Emodels.Smtppassword = EVM.Smtppassword;
                    Emodels.Smtp = EVM.Stmp;
                    Emodels.Port = EVM.Port;
                    bool sendemail = comman.SendEmail(Emodels);
                    if (sendemail == true)
                    {
                        return Json(new { error = false, message = "Confirm" });
                    }
                    else
                    {
                        return Json(new { error = false, message = "Invaild" });
                    }

                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            return Json(new { error = true, message = "Error" });

            // If we got this far, something failed, redisplay form
            //return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}