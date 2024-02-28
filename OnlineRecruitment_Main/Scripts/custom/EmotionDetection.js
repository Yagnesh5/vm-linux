Promise.all([
   faceapi.nets.tinyFaceDetector.loadFromUri('/Scripts/FaceApi'),
  faceapi.nets.faceLandmark68Net.loadFromUri('/Scripts/FaceApi'),
  faceapi.nets.faceRecognitionNet.loadFromUri('/Scripts/FaceApi'),
  faceapi.nets.faceExpressionNet.loadFromUri('/Scripts/FaceApi')
]).then()
        

