$(document).ready(function () {
    $('.datepicker').datepicker({
        format: 'd-M-yyyy'
    });
    var coll = document.getElementsByClassName("mycollapsible");
    var i;

    for (i = 0; i < coll.length; i++) {
        coll[i].addEventListener("click", function () {
            this.classList.toggle("myactive");
            var content = this.nextElementSibling;
            if (content.style.display === "block") {
                content.style.display = "none";
            } else {
                content.style.display = "block";
            }
        });
    }
  
});