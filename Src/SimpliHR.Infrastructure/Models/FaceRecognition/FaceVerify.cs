using System;
using System.Collections.Generic;
using System.Text;

namespace SimpliHR.Infrastructure.FaceRecognition;

public class FaceVerify
{
    public short matchResult { get; set; }
    public FaceDetect image1_face { get; set; }
    public FaceDetect image2_face { get; set; }
}
