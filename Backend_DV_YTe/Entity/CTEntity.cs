namespace Backend_DV_YTe.Entity
{
    public abstract class CTEntity
    {
        protected CTEntity()
        {
            //Id = Guid.NewGuid().ToString("N");
            CreateTimes = DateTime.Now;

        }
        public DateTime? CreateTimes { get; set; }
        public int? CreateBy { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
