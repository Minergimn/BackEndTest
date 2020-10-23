namespace BackEndTest.Implementations
{
    internal partial class  ListNodeJsonSerializer
    {
        private const string FileStart = "{ \"count\": ";
        private const string ElementsStart = ", \"elements\": [";
        private const string FileFinish = "]}";
        private const string ElementStart = "{\"data\": \"";
        private const string ElementMiddle = "\", \"randomNodeNumber\": ";
        private const string ElementFinish = "}";
        private const string ElementSeparator = ",";

        private const int CountIndex = 2;
        private const int DataIndex = 1;
        private const int RandomNodeNumberIndex = 3;
        private const int NullRandomNodeNumber = -1;
    }
}