namespace Backend_DV_YTe.Entity
{
    public abstract class CTEntity
    {
        public int? CreateBy { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
