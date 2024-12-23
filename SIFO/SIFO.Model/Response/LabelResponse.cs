namespace SIFO.Model.Response
{
    public class LabelResponse
    {
        public LanguageData en { get; set; }
        public LanguageData it { get; set; }
    }

    public class LanguageData
    {
        public Dictionary<string, string> labels { get; set; }
    }
}
