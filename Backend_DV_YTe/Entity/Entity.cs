using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_DV_YTe.Entity
{
    public abstract class Entity
    {
        protected  Entity()
        {
            //Id = Guid.NewGuid().ToString("N");
            CreateTimes = DateTime.Now;

        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreateTimes { get; set; }
        public int? CreateBy { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
