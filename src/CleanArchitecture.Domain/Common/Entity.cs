using System;

namespace CleanArchitecture.Domain.Common
{
    public abstract class Entity
    {
        public virtual Guid Id { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
