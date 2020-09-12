namespace AntiHarassment.Contract
{
    public class MarkSuspensionValidityModel
    {
        public bool Invalidate { get; set; }
        public string InvalidationReason { get; set; }
    }
}
