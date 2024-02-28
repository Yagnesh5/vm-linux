////const video = document.getElementById('video')
//let predictedAges = [];

var emotions = [];
//var strEmotions = "";

Promise.all([
   faceapi.nets.tinyFaceDetector.loadFromUri('/Scripts/FaceApi'),
  faceapi.nets.faceLandmark68Net.loadFromUri('/Scripts/FaceApi'),
  faceapi.nets.faceRecognitionNet.loadFromUri('/Scripts/FaceApi'),
  faceapi.nets.faceExpressionNet.loadFromUri('/Scripts/FaceApi')
]).then()

//function startVideo() {
//    navigator.getUserMedia(
//      { video: {} },
//      stream => video.srcObject = stream,
//      err => console.error(err)
//    )
//}

video.addEventListener('play', () => {

    const canvas = faceapi.createCanvasFromMedia(video)
    document.body.append(canvas)
    const displaySize = { width: video.width, height: video.height }
    faceapi.matchDimensions(canvas, displaySize)
    emotions = [];
    setInterval(async () => {
        //const detections = await faceapi.detectAllFaces(video, new faceapi.TinyFaceDetectorOptions()).withFaceLandmarks().withFaceExpressions()
        const detections = await faceapi.detectSingleFace(video, new faceapi.TinyFaceDetectorOptions())
     .withFaceLandmarks()
     .withFaceExpressions();

        const resizedDetections = faceapi.resizeResults(detections, displaySize)
        canvas.getContext('2d').clearRect(0, 0, canvas.width, canvas.height)
        faceapi.draw.drawDetections(canvas, resizedDetections)
        faceapi.draw.drawFaceLandmarks(canvas, resizedDetections)
        faceapi.draw.drawFaceExpressions(canvas, resizedDetections)

        if (resizedDetections && Object.keys(resizedDetections).length > 0) {
            const age = resizedDetections.age;
            //const interpolatedAge = interpolateAgePredictions(age);
            const gender = resizedDetections.gender;
            const expressions = resizedDetections.expressions;
            console.log(expressions);
            console.log(parseFloat(video.currentTime.toFixed(2)));
            saveEmotions(expressions);
            const maxValue = Math.max(...Object.values(expressions));
            const emotion = Object.keys(expressions).filter(
              item => expressions[item] === maxValue
            );
            var duration = video.duration;
            var time = video.currentTime;
            //document.getElementById("age").innerText = `Age - ${interpolatedAge}`;
            //document.getElementById("gender").innerText = `Gender - ${gender}`;
            document.getElementById("emotion").innerText = `Emotion - ${emotion[0]}`;

        }

    }, 3000)

    function interpolateAgePredictions(age) {
        predictedAges = [age].concat(predictedAges).slice(0, 30);
        const avgPredictedAge =
          predictedAges.reduce((total, a) => total + a) / predictedAges.length;
        return avgPredictedAge;
    }

    function saveEmotions(expressions) {
        if (parseFloat(expressions.neutral.toFixed(2)) > 0.00){
            emotions.push({
                EmotionName: "Neutral", EmotionScore: expressions.neutral.toFixed(2), EmotionTime: parseFloat(video.currentTime.toFixed(2))
            });
            // strEmotions += "{EmotionName : Neutral ,EmotionScore:" + expressions.neutral.toFixed(2)+ " ,EmotionTime: "+parseFloat(video.currentTime.toFixed(2))+"},";
        }
        if(parseFloat(expressions.happy.toFixed(2)) > 0.00){            
            emotions.push({
                EmotionName: "Happy", EmotionScore: expressions.happy.toFixed(2), EmotionTime: parseFloat(video.currentTime.toFixed(2))
            });
            // strEmotions += "{EmotionName : Happy ,EmotionScore:" + expressions.happy.toFixed(2) + " ,EmotionTime: " +parseFloat(video.currentTime.toFixed(2))+"},";
        }
        if(parseFloat(expressions.sad.toFixed(2)) > 0.00){
            emotions.push({
                EmotionName: "Sad", EmotionScore: expressions.sad.toFixed(2), EmotionTime: parseFloat(video.currentTime.toFixed(2))
            });
            // strEmotions += "{EmotionName : Sad ,EmotionScore:" + expressions.sad.toFixed(2) + " ,EmotionTime: " +parseFloat(video.currentTime.toFixed(2))+"},";
        }
        if(parseFloat(expressions.angry.toFixed(2)) > 0.00){
            emotions.push({
                EmotionName: "Angry", EmotionScore: expressions.angry.toFixed(2), EmotionTime: parseFloat(video.currentTime.toFixed(2))
            });
            // strEmotions += "{EmotionName : Angry ,EmotionScore:" + expressions.angry.toFixed(2) + " ,EmotionTime: " +parseFloat(video.currentTime.toFixed(2))+"},";
        }
        if(parseFloat(expressions.fearful.toFixed(2)) > 0.00){
            emotions.push({
                EmotionName: "Fearful", EmotionScore: expressions.fearful.toFixed(2), EmotionTime: parseFloat(video.currentTime.toFixed(2))
            });
            // strEmotions += "{EmotionName : Fearful ,EmotionScore:" + expressions.fearful.toFixed(2) + " ,EmotionTime: " +parseFloat(video.currentTime.toFixed(2))+"},";
        }
        if(parseFloat(expressions.disgusted.toFixed(2)) > 0.00){
            emotions.push({
                EmotionName: "Disgusted", EmotionScore: expressions.disgusted.toFixed(2), EmotionTime: parseFloat(video.currentTime.toFixed(2))
            });
            // strEmotions += "{EmotionName : Disgusted ,EmotionScore:" + expressions.disgusted.toFixed(2) + " ,EmotionTime: " +parseFloat(video.currentTime.toFixed(2))+"},";
        }
        if(parseFloat(expressions.surprised.toFixed(2)) > 0.00){
            emotions.push({
                EmotionName: "Surprised", EmotionScore: expressions.surprised.toFixed(2), EmotionTime: parseFloat(video.currentTime.toFixed(2))
            });
            // strEmotions += "{EmotionName : Surprised ,EmotionScore:" + expressions.surprised.toFixed(2) + " ,EmotionTime: " +parseFloat(video.currentTime.toFixed(2))+"},";
        }
    }
})

$(document).ready(function () {
    candidatevideodata();
})

//$(document).on("click", "#btnsubmit", function () {
//    var dataToSend = JSON.stringify({ 'lstEmotions': emotions });
//    $.ajax({
//        contentType: 'application/json; charset=utf-8',
//        dataType: 'json',
//        type: "POST",
//        url: '/Master/SaveEmotions',
//        data: dataToSend,
//        success: function (result) {
//            if (result.Error === true) {
//                toastr.error('Error - ' + result.Message);
//            } else {


//                toastr.success('Success - Data Inserted Successfully...!!!');

//            }
//            $("html, body").animate({ scrollTop: 0 }, 600);
//        },
//        error: function (err) {
//            toastr.error(err.statusText);
//        }
//    });
//});




function candidatevideodata(videourl){
    $("#video").html('');
    $("#video").html('<source src="../../InterviewVideo/Client/4_CandidateVideo_768_RecordRTC-2020412-c5tzurxktoc.webm" type="video/mp4">');
}


