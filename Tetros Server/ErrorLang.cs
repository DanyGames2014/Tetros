namespace Tetros
{
    public class ErrorLang
    {
        public string[] errorLang = new string[500];

        public ErrorLang()
        {
            // Block 0 - 99 | General Errors
            errorLang[0] = "Unknown Error";

            // Block 100 - 199 | Frame Errors
            errorLang[100] = "Invalid Frame Data";
        }
    }
}
