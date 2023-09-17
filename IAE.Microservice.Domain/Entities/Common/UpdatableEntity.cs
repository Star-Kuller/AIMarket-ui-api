using System;
using IAE.Microservice.Common;

namespace IAE.Microservice.Domain.Entities.Common
{
    public class UpdatableEntity : Entity
    {
        public UpdatableEntity()
        {
            Status = Status.Active;
        }

        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; protected set; }
        public Status Status { get; set; }

        public bool IsDeleted => Status == Status.Deleted;

        public virtual void Activate() => Status = Status.Active;

        public virtual void Ban() => Status = Status.Banned;

        public virtual void Delete()
        {
            Status = Status.Deleted;
            DeletedAt = SystemTime.DateTime.UtcNow;
        }

        public void ChangeStatus(Status newStatus)
        {
            switch (newStatus)
            {
                case Status.Active:
                    Activate();
                    break;
                case Status.Banned:
                    Ban();
                    break;
                case Status.Deleted:
                    Delete();
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }


    }
}
