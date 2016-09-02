
namespace LinqToQueryString.UnitTests.Utils
{
    using System;

    public class GuidWrapper : IEquatable<GuidWrapper>
    {
        public GuidWrapper(Guid id)
        {
            this.Id = id;
        }

        public static explicit operator Guid(GuidWrapper x)
        {
            if (x == null)
            {
                throw new InvalidCastException("Cannot cast null to Guid");
            }

            return x.Id;
        }

        public static explicit operator Guid? (GuidWrapper x)
        {
            return x?.Id;
        }

        public static explicit operator GuidWrapper(Guid x)
        {
            return new GuidWrapper(x);
        }

        public static explicit operator GuidWrapper(Guid? x)
        {
            if (x == null)
            {
                return null;
            }

            return new GuidWrapper(x.Value);
        }

        public static bool operator ==(GuidWrapper left, GuidWrapper right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GuidWrapper left, GuidWrapper right)
        {
            return !Equals(left, right);
        }

        public Guid Id { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GuidWrapper);
        }

        /// <inheritdoc/>
        public bool Equals(GuidWrapper other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Id.Equals(other.Id);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}