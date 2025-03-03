using System;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Luxand
{
    public unsafe class FSDKCam
    {
#if IOS
        private const string Dll = "__Internal";
#else
        private const string Dll = "facesdk";
#endif

        public struct VideoFormatInfo{
            public int Width;
            public int Height;
            public int BPP;
        }

        public enum FSDK_VIDEOCOMPRESSIONTYPE{
            FSDK_MJPEG = 0
        }

        private static bool allZero<T>(in T value) where T : unmanaged
        {
            fixed(T* pointer = &value)
            {
                var p = (byte*) pointer;
                for (var i = 0; i < sizeof(T); ++i)
                    if (p[i] != 0)
                        return false;
            }

            return true;
        }


        private static int GetLength<T>(T* data) where T : unmanaged
        {
            var res = 0;
            while (!allZero(data[res]))
            {
                ++res;
            }

            return sizeof(T) * res;
        }

        private static bool IsUTF16()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.WinCE:
                case PlatformID.Win32S:
                case PlatformID.Win32NT:
                case PlatformID.Win32Windows:
                    return true;
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    return false;
                case PlatformID.Xbox:
                    throw new InvalidEnumArgumentException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static byte[] GetByteArray(byte* data)
        {
            return new byte[IsUTF16() ? GetLength((char*)data) : GetLength(data)];
        }

        private static string[] GetStrings(byte** data, int count)
        {
            var encoding = IsUTF16() ? Encoding.Unicode : Encoding.UTF8;
            var result = new string[count];
            for (var i = 0; i < count; ++i)
            {
                var bytes = GetByteArray(data[i]);
                Marshal.Copy((IntPtr)data[i], bytes, 0, bytes.Length);
                result[i] = encoding.GetString(bytes);
            }

            return result;
        }

        private static byte[] EncodeString(string value)
        {
            return IsUTF16() ? Encoding.Unicode.GetBytes(value) : Encoding.UTF8.GetBytes(value);
        }

        [DllImport(Dll, EntryPoint = "FSDK_InitializeCapturing", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitializeCapturing();

        [DllImport(Dll, EntryPoint = "FSDK_FinalizeCapturing", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FinalizeCapturing();

        [DllImport(Dll, EntryPoint = "FSDK_SetCameraNaming", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetCameraNaming(bool UseDevicePathAsName);

        [DllImport(Dll, EntryPoint = "FSDK_FreeCameraList", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_FreeCameraList(byte** CameraList, int CameraCount);
        
        [DllImport(Dll, EntryPoint = "FSDK_GetCameraList", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetCameraList_Old(byte*** CameraList, out int CameraCount);
        public static int GetCameraList(out string[] CameraList, out int CameraCount){
            byte** pCameraList;
            int res = FSDK_GetCameraList_Old(&pCameraList, out CameraCount);
            CameraList = GetStrings(pCameraList, CameraCount);
            FSDK_FreeCameraList(pCameraList, CameraCount);
            return res;
        }

        [DllImport(Dll, EntryPoint = "FSDK_GetCameraListEx", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetCameraListEx_Old(byte*** CameraNameList, byte*** CameraDevicePathList, out int CameraCount);
        public static int GetCameraListEx(out string[] CameraNameList, out string[] CameraDevicePathList, out int CameraCount)
        {
            byte** pCameraNMList;
            byte** pCameraDPList;
            var res = FSDK_GetCameraListEx_Old(&pCameraNMList, &pCameraDPList, out CameraCount);
            CameraNameList = GetStrings(pCameraNMList, CameraCount);
            CameraDevicePathList = GetStrings(pCameraDPList, CameraCount);
            FSDK_FreeCameraList(pCameraNMList, CameraCount);
            FSDK_FreeCameraList(pCameraDPList, CameraCount);
            return res;
        }

        [DllImport(Dll, EntryPoint = "FSDK_FreeVideoFormatList", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_FreeVideoFormatList(void* VideoFormatList);

        [DllImport(Dll, EntryPoint = "FSDK_GetVideoFormatList", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetVideoFormatList_Old([In]byte[] CameraName, out void* VideoFormatList, out int VideoFormatCount);
        public static int GetVideoFormatList(ref string CameraName, out VideoFormatInfo[] VideoFormatList, out int VideoFormatCount){
            var res = FSDK_GetVideoFormatList_Old(EncodeString(CameraName), out var pVideoFormatList, out VideoFormatCount);
            VideoFormatList = new VideoFormatInfo[VideoFormatCount];
            for (var i = 0; i < VideoFormatCount; ++i)
            {
                VideoFormatList[i] = ((VideoFormatInfo*)pVideoFormatList)[i];
            }
            FSDK_FreeVideoFormatList(pVideoFormatList);
            return res;
        }

        [DllImport(Dll, EntryPoint = "FSDK_SetVideoFormat", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_SetVideoFormat_Old([In]byte[] CameraName, VideoFormatInfo VideoFormat);
        public static int SetVideoFormat(ref string CameraName, VideoFormatInfo VideoFormat){
            return FSDK_SetVideoFormat_Old(EncodeString(CameraName), VideoFormat);
        }

        [DllImport(Dll, EntryPoint = "FSDK_OpenVideoCamera", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_OpenVideoCamera_Old([In]byte[] CameraName, ref int CameraHandle);
        public static int OpenVideoCamera(ref string CameraName, ref int CameraHandle){
            return FSDK_OpenVideoCamera_Old(EncodeString(CameraName), ref CameraHandle);
        }

        [DllImport(Dll, EntryPoint = "FSDK_OpenIPVideoCamera", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenIPVideoCamera(FSDK_VIDEOCOMPRESSIONTYPE CompressionType, string URL, string Username, string Password, int TimeoutSeconds, ref int CameraHandle);
                
        [DllImport(Dll, EntryPoint = "FSDK_CloseVideoCamera", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseVideoCamera(int CameraHandle);

        [DllImport(Dll, EntryPoint = "FSDK_GrabFrame", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GrabFrame(int CameraHandle, ref int Image);
    }





    public unsafe partial class FSDK
    {
#if IOS
        private const string Dll = "__Internal";
#else
        private const string Dll = "facesdk";
#endif

        //TYPES AND CONSTANTS{
        public const int TemplateSize = 1040;
        public const int sizeofTFacePosition = 24;

        
        public const int FSDKE_OK = 0;
        public const int FSDKE_FAILED = -1;
        public const int FSDKE_NOT_ACTIVATED = -2;
        public const int FSDKE_OUT_OF_MEMORY = -3;
        public const int FSDKE_INVALID_ARGUMENT = -4;
        public const int FSDKE_IO_ERROR = -5;
        public const int FSDKE_IMAGE_TOO_SMALL = -6;
        public const int FSDKE_FACE_NOT_FOUND = -7;
        public const int FSDKE_INSUFFICIENT_BUFFER_SIZE = -8;
        public const int FSDKE_UNSUPPORTED_IMAGE_EXTENSION = -9;
        public const int FSDKE_CANNOT_OPEN_FILE = -10;
        public const int FSDKE_CANNOT_CREATE_FILE = -11;
        public const int FSDKE_BAD_FILE_FORMAT = -12;
        public const int FSDKE_FILE_NOT_FOUND = -13;
        public const int FSDKE_CONNECTION_CLOSED = -14;
        public const int FSDKE_CONNECTION_FAILED = -15;
        public const int FSDKE_IP_INIT_FAILED = -16;
        public const int FSDKE_NEED_SERVER_ACTIVATION = -17;
        public const int FSDKE_ID_NOT_FOUND = -18;
        public const int FSDKE_ATTRIBUTE_NOT_DETECTED = -19;
        public const int FSDKE_INSUFFICIENT_TRACKER_MEMORY_LIMIT = -20;
        public const int FSDKE_UNKNOWN_ATTRIBUTE = -21;
        public const int FSDKE_UNSUPPORTED_FILE_VERSION = -22;
        public const int FSDKE_SYNTAX_ERROR = -23;
        public const int FSDKE_PARAMETER_NOT_FOUND = -24;
        public const int FSDKE_INVALID_TEMPLATE = -25;
        public const int FSDKE_UNSUPPORTED_TEMPLATE_VERSION = -26;
        public const int FSDKE_CAMERA_INDEX_DOES_NOT_EXIST = -27;
        public const int FSDKE_PLATFORM_NOT_LICENSED = -28;


        
        public const int FSDK_FACIAL_FEATURE_COUNT = 70;

        public enum FSDK_IMAGEMODE{
            FSDK_IMAGE_GRAYSCALE_8BIT,
            FSDK_IMAGE_COLOR_24BIT,
            FSDK_IMAGE_COLOR_32BIT
        };

        public struct TPoint{
            public int x, y;
        }

        public struct TFacePosition{
            public int xc, yc, w;
            public int padding;
            public double angle;
        }

        public enum FacialFeatures {
            FSDKP_LEFT_EYE = 0,
            FSDKP_RIGHT_EYE = 1,
            FSDKP_LEFT_EYE_INNER_CORNER = 24,
            FSDKP_LEFT_EYE_OUTER_CORNER = 23,
            FSDKP_LEFT_EYE_LOWER_LINE1 = 38,
            FSDKP_LEFT_EYE_LOWER_LINE2 = 27,
            FSDKP_LEFT_EYE_LOWER_LINE3 = 37,
            FSDKP_LEFT_EYE_UPPER_LINE1 = 35,
            FSDKP_LEFT_EYE_UPPER_LINE2 = 28,
            FSDKP_LEFT_EYE_UPPER_LINE3 = 36,
            FSDKP_LEFT_EYE_LEFT_IRIS_CORNER = 29,
            FSDKP_LEFT_EYE_RIGHT_IRIS_CORNER = 30,
            FSDKP_RIGHT_EYE_INNER_CORNER = 25,
            FSDKP_RIGHT_EYE_OUTER_CORNER = 26,
            FSDKP_RIGHT_EYE_LOWER_LINE1 = 41,
            FSDKP_RIGHT_EYE_LOWER_LINE2 = 31,
            FSDKP_RIGHT_EYE_LOWER_LINE3 = 42,
            FSDKP_RIGHT_EYE_UPPER_LINE1 = 40,
            FSDKP_RIGHT_EYE_UPPER_LINE2 = 32,
            FSDKP_RIGHT_EYE_UPPER_LINE3 = 39,
            FSDKP_RIGHT_EYE_LEFT_IRIS_CORNER = 33,
            FSDKP_RIGHT_EYE_RIGHT_IRIS_CORNER = 34,
            FSDKP_LEFT_EYEBROW_INNER_CORNER = 13,
            FSDKP_LEFT_EYEBROW_MIDDLE = 16,
            FSDKP_LEFT_EYEBROW_MIDDLE_LEFT = 18,
            FSDKP_LEFT_EYEBROW_MIDDLE_RIGHT = 19,
            FSDKP_LEFT_EYEBROW_OUTER_CORNER = 12,
            FSDKP_RIGHT_EYEBROW_INNER_CORNER = 14,
            FSDKP_RIGHT_EYEBROW_MIDDLE = 17,
            FSDKP_RIGHT_EYEBROW_MIDDLE_LEFT = 20,
            FSDKP_RIGHT_EYEBROW_MIDDLE_RIGHT = 21,
            FSDKP_RIGHT_EYEBROW_OUTER_CORNER = 15,
            FSDKP_NOSE_TIP = 2,
            FSDKP_NOSE_BOTTOM = 49,
            FSDKP_NOSE_BRIDGE = 22,
            FSDKP_NOSE_LEFT_WING = 43,
            FSDKP_NOSE_LEFT_WING_OUTER = 45,
            FSDKP_NOSE_LEFT_WING_LOWER = 47,
            FSDKP_NOSE_RIGHT_WING = 44,
            FSDKP_NOSE_RIGHT_WING_OUTER = 46,
            FSDKP_NOSE_RIGHT_WING_LOWER = 48,
            FSDKP_MOUTH_RIGHT_CORNER = 3,
            FSDKP_MOUTH_LEFT_CORNER = 4,
            FSDKP_MOUTH_TOP = 54,
            FSDKP_MOUTH_TOP_INNER = 61,
            FSDKP_MOUTH_BOTTOM = 55,
            FSDKP_MOUTH_BOTTOM_INNER = 64,
            FSDKP_MOUTH_LEFT_TOP = 56,
            FSDKP_MOUTH_LEFT_TOP_INNER = 60,
            FSDKP_MOUTH_RIGHT_TOP = 57,
            FSDKP_MOUTH_RIGHT_TOP_INNER = 62,
            FSDKP_MOUTH_LEFT_BOTTOM = 58,
            FSDKP_MOUTH_LEFT_BOTTOM_INNER = 63,
            FSDKP_MOUTH_RIGHT_BOTTOM = 59,
            FSDKP_MOUTH_RIGHT_BOTTOM_INNER = 65,
            FSDKP_NASOLABIAL_FOLD_LEFT_UPPER = 50,
            FSDKP_NASOLABIAL_FOLD_LEFT_LOWER = 52,
            FSDKP_NASOLABIAL_FOLD_RIGHT_UPPER = 51,
            FSDKP_NASOLABIAL_FOLD_RIGHT_LOWER = 53,
            FSDKP_CHIN_BOTTOM = 11,
            FSDKP_CHIN_LEFT = 9,
            FSDKP_CHIN_RIGHT = 10,
            FSDKP_FACE_CONTOUR1 = 7,
            FSDKP_FACE_CONTOUR2 = 5,
            FSDKP_FACE_CONTOUR12 = 6,
            FSDKP_FACE_CONTOUR13 = 8,
            FSDKP_FACE_CONTOUR14 = 66,
            FSDKP_FACE_CONTOUR15 = 67,
            FSDKP_FACE_CONTOUR16 = 68,
            FSDKP_FACE_CONTOUR17 = 69
        }
        //}



        //INITIALIZATION FUNCTIONS{
        [DllImport(Dll, EntryPoint = "FSDK_ActivateLibrary", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ActivateLibrary(string LicenseKey);
        
        [DllImport(Dll, EntryPoint = "FSDK_GetHardware_ID", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetHardware_ID_Old([OutAttribute] StringBuilder HardwareID);
        public static int GetHardware_ID(out string HardwareID){
            StringBuilder tmps = new StringBuilder(1024);
            int res = FSDK_GetHardware_ID_Old(tmps);
            HardwareID = tmps.ToString();
            return res;
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_GetLicenseInfo", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetLicenseInfo_Old([OutAttribute] StringBuilder LicenseInfo);
        public static int GetLicenseInfo(out string LicenseInfo){
            StringBuilder tmps = new StringBuilder(1024);
            int res = FSDK_GetLicenseInfo_Old(tmps);
            LicenseInfo = tmps.ToString();
            return res;
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_Initialize", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_Initialize_Old(string DataFilesPath);
        public static int InitializeLibrary(){
            return FSDK_Initialize_Old("");
        }

        [DllImport(Dll, EntryPoint = "FSDK_Finalize", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FinalizeLibrary();

        [DllImport(Dll, EntryPoint = "FSDK_SetHTTPProxy", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetHTTPProxy(string ServerNameOrIPAddress, UInt16 Port, string UserName, string Password);

        [DllImport(Dll, EntryPoint = "FSDK_SetNumThreads", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetNumThreads(int Num);

        [DllImport(Dll, EntryPoint = "FSDK_GetNumThreads", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetNumThreads(ref int Num);
        //}

        //FACE DETECTION FUNCTIONS{
        [DllImport(Dll, EntryPoint = "FSDK_DetectEyes", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_DetectEyes_Old(int Image, [Out, MarshalAs(UnmanagedType.LPArray)] TPoint[] FacialFeatures);
        public static int DetectEyes(int Image, out TPoint[] FacialFeatures){
            FacialFeatures = new TPoint[FSDK.FSDK_FACIAL_FEATURE_COUNT];
            return FSDK_DetectEyes_Old(Image, FacialFeatures);
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_DetectEyesInRegion", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_DetectEyesInRegion_Old(int Image, ref TFacePosition FacePosition, [Out, MarshalAs(UnmanagedType.LPArray)] TPoint[] FacialFeatures);
        public static int DetectEyesInRegion(int Image, ref TFacePosition FacePosition, out TPoint[] FacialFeatures){
            FacialFeatures = new TPoint[FSDK.FSDK_FACIAL_FEATURE_COUNT];
            return FSDK_DetectEyesInRegion_Old(Image, ref FacePosition, FacialFeatures);
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_DetectFace", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DetectFace(int Image, ref TFacePosition FacePosition);

        [DllImport(Dll, EntryPoint = "FSDK_DetectMultipleFaces", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_DetectMultipleFaces_Old(int Image, ref int DetectedCount, [Out, MarshalAs(UnmanagedType.LPArray)] TFacePosition[] FaceArray, int MaxSizeInBytes);
        public static int DetectMultipleFaces(int Image, ref int DetectedCount, out TFacePosition[] FaceArray, int MaxSizeInBytes){
            FaceArray = new TFacePosition[MaxSizeInBytes / sizeofTFacePosition];
            return FSDK_DetectMultipleFaces_Old(Image, ref DetectedCount, FaceArray, MaxSizeInBytes);
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_DetectFacialFeatures", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_DetectFacialFeatures_Old(int Image, [Out, MarshalAs(UnmanagedType.LPArray)] TPoint[] FacialFeatures);
        public static int DetectFacialFeatures(int Image, out TPoint[] FacialFeatures){
            FacialFeatures = new TPoint[FSDK.FSDK_FACIAL_FEATURE_COUNT];
            return FSDK_DetectFacialFeatures_Old(Image, FacialFeatures);
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_DetectFacialFeaturesInRegion", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_DetectFacialFeaturesInRegion_Old(int Image, ref TFacePosition FacePosition, [Out, MarshalAs(UnmanagedType.LPArray)] TPoint[] FacialFeatures);
        public static int DetectFacialFeaturesInRegion(int Image, ref TFacePosition FacePosition, out TPoint[] FacialFeatures){
            FacialFeatures = new TPoint[FSDK.FSDK_FACIAL_FEATURE_COUNT];
            return FSDK_DetectFacialFeaturesInRegion_Old(Image, ref FacePosition, FacialFeatures);
        }

        [DllImport(Dll, EntryPoint = "FSDK_SetFaceDetectionParameters", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetFaceDetectionParameters(bool HandleArbitraryRotations, bool DetermineFaceRotationAngle, int InternalResizeWidth);

        [DllImport(Dll, EntryPoint = "FSDK_SetFaceDetectionThreshold", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetFaceDetectionThreshold(int Threshold);
        //}

        //IMAGE MANIPULATION FUNCTIONS{
        [DllImport(Dll, EntryPoint = "FSDK_CreateEmptyImage", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateEmptyImage(ref int Image);

        [DllImport(Dll, EntryPoint = "FSDK_LoadImageFromBuffer", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadImageFromBuffer(ref int Image, byte[] Buffer, int Width, int Height, int ScanLine, FSDK_IMAGEMODE ImageMode);


        [DllImport(Dll, EntryPoint = "FSDK_GetImageBufferSize", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetImageBufferSize(int Image, ref int BufSize, FSDK_IMAGEMODE ImageMode);

        [DllImport(Dll, EntryPoint = "FSDK_SaveImageToBuffer", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_SaveImageToBuffer_Old(int Image, [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] Buffer, FSDK_IMAGEMODE ImageMode);
        public static int SaveImageToBuffer(int Image, out byte[] Buffer, FSDK_IMAGEMODE ImageMode)
        {
            int MaxSizeInBytes = 0;
            int res = GetImageBufferSize(Image, ref MaxSizeInBytes, ImageMode);
            if (res != FSDKE_OK){
                Buffer = null;
                return res;
            }
            Buffer = new byte[MaxSizeInBytes];
            return FSDK_SaveImageToBuffer_Old(Image, Buffer, ImageMode);
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_LoadImageFromFile", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadImageFromFile(ref int Image, string FileName);

        [DllImport(Dll, EntryPoint = "FSDK_LoadImageFromFileW", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadImageFromFileW(ref int Image, [In, MarshalAs(UnmanagedType.BStr)]string FileName);

        [DllImport(Dll, EntryPoint = "FSDK_FreeImage", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FreeImage(int Image);
        
        [DllImport(Dll, EntryPoint = "FSDK_SaveImageToFile", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SaveImageToFile(int Image, string FileName);

        [DllImport(Dll, EntryPoint = "FSDK_SaveImageToFileW", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SaveImageToFileW(int Image, [In, MarshalAs(UnmanagedType.BStr)]string FileName);

        [DllImport(Dll, EntryPoint = "FSDK_LoadImageFromHBitmap", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadImageFromHBitmap(ref int Image, IntPtr BitmapHandle);

        [DllImport(Dll, EntryPoint = "FSDK_SaveImageToHBitmap", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SaveImageToHBitmap(int Image, ref IntPtr BitmapHandle);

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

#if USE_SYSTEM_DRAWING
        public static int LoadImageFromCLRImage(ref int Image, System.Drawing.Image ImageObject){
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(ImageObject);
            IntPtr hbm = bmp.GetHbitmap();
            int res = LoadImageFromHBitmap(ref Image, hbm);
            DeleteObject(hbm);
            bmp.Dispose();
            return res;
        }

        public static int SaveImageToCLRImage(int Image, ref System.Drawing.Image ImageObject){
            IntPtr hbm = IntPtr.Zero;
            int res = SaveImageToHBitmap(Image, ref hbm);
            ImageObject = System.Drawing.Image.FromHbitmap(hbm);
            DeleteObject(hbm);
            return res;
        }
#endif

        [DllImport(Dll, EntryPoint = "FSDK_SetJpegCompressionQuality", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetJpegCompressionQuality(int Quality);

        [DllImport(Dll, EntryPoint = "FSDK_CopyImage", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CopyImage(int SourceImage, int DestImage);

        [DllImport(Dll, EntryPoint = "FSDK_ResizeImage", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ResizeImage(int SourceImage, double ratio, int DestImage);

        [DllImport(Dll, EntryPoint = "FSDK_MirrorImage", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MirrorImage(int Image, bool UseVerticalMirroringInsteadOfHorizontal);
        
        [DllImport(Dll, EntryPoint = "FSDK_RotateImage", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RotateImage(int SourceImage, double angle, int DestImage);

        [DllImport(Dll, EntryPoint = "FSDK_RotateImageCenter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RotateImageCenter(int SourceImage, double angle, double xCenter, double yCenter, int DestImage);

        [DllImport(Dll, EntryPoint = "FSDK_RotateImage90", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RotateImage90(int SourceImage, int Multiplier, int DestImage);

        [DllImport(Dll, EntryPoint = "FSDK_CopyRect", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CopyRect(int SourceImage, int x1, int y1, int x2, int y2, int DestImage);

        [DllImport(Dll, EntryPoint = "FSDK_CopyRectReplicateBorder", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CopyRectReplicateBorder(int SourceImage, int x1, int y1, int x2, int y2, int DestImage);

        [DllImport(Dll, EntryPoint = "FSDK_GetImageWidth", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetImageWidth(int SourceImage, ref int Width);

        [DllImport(Dll, EntryPoint = "FSDK_GetImageHeight", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetImageHeight(int SourceImage, ref int Height);

        [DllImport(Dll, EntryPoint = "FSDK_ExtractFaceImage", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_ExtractFaceImage_Old(int Image, [In, MarshalAs(UnmanagedType.LPArray)] TPoint[] FacialFeatures, int Width, int Height, ref int ExtractedFaceImage, [Out, MarshalAs(UnmanagedType.LPArray)] TPoint[] ResizedFeatures);
        public static int ExtractFaceImage(int Image, ref TPoint[] FacialFeatures, int Width, int Height, ref int ExtractedFaceImage, out TPoint[] ResizedFeatures){
            ResizedFeatures = new TPoint[FSDK.FSDK_FACIAL_FEATURE_COUNT];
            return FSDK_ExtractFaceImage_Old(Image, FacialFeatures, Width, Height, ref ExtractedFaceImage, ResizedFeatures);
        }
        //}
      
        //MATCHING{
        [DllImport(Dll, EntryPoint = "FSDK_GetFaceTemplate", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetFaceTemplate_Old(int Image, [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] FaceTemplate);
        public static int GetFaceTemplate(int Image, out byte[] FaceTemplate){
            FaceTemplate = new byte[FSDK.TemplateSize];
            return FSDK_GetFaceTemplate_Old(Image, FaceTemplate);
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_GetFaceTemplateInRegion", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetFaceTemplateInRegion_Old(int Image, ref TFacePosition FacePosition, [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] FaceTemplate);
        public static int GetFaceTemplateInRegion(int Image, ref TFacePosition FacePosition, out byte[] FaceTemplate){
            FaceTemplate = new byte[FSDK.TemplateSize];
            return FSDK_GetFaceTemplateInRegion_Old(Image, ref FacePosition, FaceTemplate);
        }

        [DllImport(Dll, EntryPoint = "FSDK_GetFaceTemplateUsingEyes", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetFaceTemplateUsingEyes_Old(int Image, [In, Out, MarshalAs(UnmanagedType.LPArray)] TPoint[] eyeCoords, [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] FaceTemplate);
        public static int GetFaceTemplateUsingEyes(int Image, ref TPoint[] eyeCoords, out byte[] FaceTemplate){
            FaceTemplate = new byte[FSDK.TemplateSize];
            return FSDK_GetFaceTemplateUsingEyes_Old(Image, eyeCoords, FaceTemplate);
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_GetFaceTemplateUsingFeatures", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetFaceTemplateUsingFeatures_Old(int Image, [In, Out, MarshalAs(UnmanagedType.LPArray)] TPoint[] FacialFeatures, [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] FaceTemplate);
        public static int GetFaceTemplateUsingFeatures(int Image, ref TPoint[] FacialFeatures, out byte[] FaceTemplate){
            FaceTemplate = new byte[FSDK.TemplateSize];
            return FSDK_GetFaceTemplateUsingFeatures_Old(Image, FacialFeatures, FaceTemplate);
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_MatchFaces", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_MatchFaces_Old([In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] FaceTemplate1, [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] FaceTemplate2, ref float Similarity);
        public static int MatchFaces(ref byte[] FaceTemplate1, ref byte[] FaceTemplate2, ref float Similarity){
            return FSDK_MatchFaces_Old(FaceTemplate1, FaceTemplate2, ref Similarity);
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_GetMatchingThresholdAtFAR", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMatchingThresholdAtFAR(float FARValue, ref float Threshold);

        [DllImport(Dll, EntryPoint = "FSDK_GetMatchingThresholdAtFRR", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMatchingThresholdAtFRR(float FRRValue, ref float Threshold);
        //}

        //TRACKER{
        [DllImport(Dll, EntryPoint = "FSDK_CreateTracker", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateTracker(ref int Tracker);

        [DllImport(Dll, EntryPoint = "FSDK_FreeTracker", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FreeTracker(int Tracker);

        [DllImport(Dll, EntryPoint = "FSDK_ClearTracker", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ClearTracker(int Tracker);

        [DllImport(Dll, EntryPoint = "FSDK_SetTrackerParameter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetTrackerParameter(int Tracker, string ParameterName, string ParameterValue);

        [DllImport(Dll, EntryPoint = "FSDK_SetTrackerMultipleParameters", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetTrackerMultipleParameters(int Tracker, string Parameters, ref int ErrorPosition);

        [DllImport(Dll, EntryPoint = "FSDK_GetTrackerParameter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetTrackerParameter_Old(int Tracker, string ParameterName, [OutAttribute] StringBuilder ParameterValue, long MaxSizeInBytes);
        public static int GetTrackerParameter(int Tracker, string ParameterName, out string ParameterValue, long MaxSizeInBytes)
        {
            StringBuilder tmps = new StringBuilder((int)MaxSizeInBytes);
            int res = FSDK_GetTrackerParameter_Old(Tracker, ParameterName, tmps, MaxSizeInBytes);
            ParameterValue = tmps.ToString();
            return res;
        }

        [DllImport(Dll, EntryPoint = "FSDK_FeedFrame", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_FeedFrame_Old(int Tracker, long CameraIdx, int Image, ref long FaceCount, [Out, MarshalAs(UnmanagedType.LPArray)] long[] IDs, long MaxSizeInBytes);
        public static int FeedFrame(int Tracker, long CameraIdx, int Image, ref long FaceCount, out long[] IDs, long MaxSizeInBytes)
        {
            IDs = new long[MaxSizeInBytes/8];
            return FSDK_FeedFrame_Old(Tracker, CameraIdx, Image, ref FaceCount, IDs, MaxSizeInBytes);
        }

        [DllImport(Dll, EntryPoint = "FSDK_GetTrackerEyes", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetTrackerEyes_Old(int Tracker, long CameraIdx, long ID, [Out, MarshalAs(UnmanagedType.LPArray)] TPoint[] FacialFeatures);
        public static int GetTrackerEyes(int Tracker, long CameraIdx, long ID, out TPoint[] FacialFeatures)
        {
            FacialFeatures = new TPoint[FSDK.FSDK_FACIAL_FEATURE_COUNT];
            return FSDK_GetTrackerEyes_Old(Tracker, CameraIdx, ID, FacialFeatures);
        }

        [DllImport(Dll, EntryPoint = "FSDK_GetTrackerFacialFeatures", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetTrackerFacialFeatures_Old(int Tracker, long CameraIdx, long ID, [Out, MarshalAs(UnmanagedType.LPArray)] TPoint[] FacialFeatures);
        public static int GetTrackerFacialFeatures(int Tracker, long CameraIdx, long ID, out TPoint[] FacialFeatures)
        {
            FacialFeatures = new TPoint[FSDK.FSDK_FACIAL_FEATURE_COUNT];
            return FSDK_GetTrackerFacialFeatures_Old(Tracker, CameraIdx, ID, FacialFeatures);
        }
        
        [DllImport(Dll, EntryPoint = "FSDK_GetTrackerFacePosition", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetTrackerFacePosition(int Tracker, long CameraIdx, long ID, ref TFacePosition FacePosition);

        [DllImport(Dll, EntryPoint = "FSDK_LockID", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int LockID(int Tracker, long ID);

        [DllImport(Dll, EntryPoint = "FSDK_UnlockID", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int UnlockID(int Tracker, long ID);

        [DllImport(Dll, EntryPoint = "FSDK_GetName", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetName_Old(int Tracker, long ID, [OutAttribute] StringBuilder Name, long MaxSizeInBytes);
        public static int GetName(int Tracker, long ID, out string Name, long MaxSizeInBytes)
        {
            StringBuilder tmps = new StringBuilder((int)MaxSizeInBytes);
            int res = FSDK_GetName_Old(Tracker, ID, tmps, MaxSizeInBytes);
            Name = tmps.ToString();
            return res;
        }

        [DllImport(Dll, EntryPoint = "FSDK_GetAllNames", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetAllNames_Old(int Tracker, long ID, [OutAttribute] StringBuilder Names, long MaxSizeInBytes);
        public static int GetAllNames(int Tracker, long ID, out string Names, long MaxSizeInBytes)
        {
            StringBuilder tmps = new StringBuilder((int)MaxSizeInBytes);
            int res = FSDK_GetAllNames_Old(Tracker, ID, tmps, MaxSizeInBytes);
            Names = tmps.ToString();
            return res;
        }

        [DllImport(Dll, EntryPoint = "FSDK_SetName", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetName(int Tracker, long ID, string Name);

        [DllImport(Dll, EntryPoint = "FSDK_PurgeID", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PurgeID(int Tracker, long ID);

        [DllImport(Dll, EntryPoint = "FSDK_GetIDReassignment", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetIDReassignment(int Tracker, long ID, ref long ReassignedID);

        [DllImport(Dll, EntryPoint = "FSDK_GetSimilarIDCount", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSimilarIDCount(int Tracker, long ID, ref long Count);

        [DllImport(Dll, EntryPoint = "FSDK_GetSimilarIDList", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetSimilarIDList_Old(int Tracker, long ID, [Out, MarshalAs(UnmanagedType.LPArray)] long[] SimilarIDList, long MaxSizeInBytes);
        public static int GetSimilarIDList(int Tracker, long ID, out long[] SimilarIDList, long MaxSizeInBytes)
        {
            SimilarIDList = new long[MaxSizeInBytes/8];
            return FSDK_GetSimilarIDList_Old(Tracker, ID, SimilarIDList, MaxSizeInBytes);
        }

        [DllImport(Dll, EntryPoint = "FSDK_SaveTrackerMemoryToFile", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SaveTrackerMemoryToFile(int Tracker, string FileName);

        [DllImport(Dll, EntryPoint = "FSDK_LoadTrackerMemoryFromFile", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadTrackerMemoryFromFile(ref int Tracker, string FileName);

        [DllImport(Dll, EntryPoint = "FSDK_GetTrackerMemoryBufferSize", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetTrackerMemoryBufferSize(int Tracker, ref long BufSize);

        [DllImport(Dll, EntryPoint = "FSDK_SaveTrackerMemoryToBuffer", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_SaveTrackerMemoryToBuffer_Old(int Tracker, [In, Out, MarshalAs(UnmanagedType.LPArray)] byte[] Buffer, long MaxSizeInBytes);
        public static int SaveTrackerMemoryToBuffer(int Tracker, out byte[] Buffer, long MaxSizeInBytes)
        {
            Buffer = new byte[MaxSizeInBytes];
            return FSDK_SaveTrackerMemoryToBuffer_Old(Tracker, Buffer, MaxSizeInBytes);
        }

        [DllImport(Dll, EntryPoint = "FSDK_LoadTrackerMemoryFromBuffer", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadTrackerMemoryFromBuffer(ref int Tracker, byte[] Buffer);
        //}

        //FACIAL_ATTRIBUTES{
        [DllImport(Dll, EntryPoint = "FSDK_GetTrackerFacialAttribute", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_GetTrackerFacialAttribute_Old(int Tracker, long CameraIdx, long ID, string AttributeName, [OutAttribute] StringBuilder AttributeValues, long MaxSizeInBytes);
        public static int GetTrackerFacialAttribute(int Tracker, long CameraIdx, long ID, string AttributeName, out string AttributeValues, long MaxSizeInBytes)
        {
            StringBuilder tmps = new StringBuilder((int)MaxSizeInBytes);
            int res = FSDK_GetTrackerFacialAttribute_Old(Tracker, CameraIdx, ID, AttributeName, tmps, MaxSizeInBytes);
            AttributeValues = tmps.ToString();
            return res;
        }

        [DllImport(Dll, EntryPoint = "FSDK_DetectFacialAttributeUsingFeatures", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int FSDK_DetectFacialAttributeUsingFeatures_Old(int Image, [In, Out, MarshalAs(UnmanagedType.LPArray)] TPoint[] FacialFeatures, string AttributeName, [OutAttribute] StringBuilder AttributeValues, long MaxSizeInBytes);
        public static int DetectFacialAttributeUsingFeatures(int Image, ref TPoint [] FacialFeatures, string AttributeName, out string AttributeValues, long MaxSizeInBytes)
        {
            StringBuilder tmps = new StringBuilder((int)MaxSizeInBytes);
            int res = FSDK_DetectFacialAttributeUsingFeatures_Old(Image, FacialFeatures, AttributeName, tmps, MaxSizeInBytes);
            AttributeValues = tmps.ToString();
            return res;
        }

        [DllImport(Dll, EntryPoint = "FSDK_GetValueConfidence", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetValueConfidence(string AttributeValues, string Value, ref float Confidence);


        [DllImport(Dll, EntryPoint = "FSDK_SetParameter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetParameter(string ParameterName, string ParameterValue);


        [DllImport(Dll, EntryPoint = "FSDK_SetParameters", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetParameters(string Parameters, out int errorPos);
        //}
    }
}
