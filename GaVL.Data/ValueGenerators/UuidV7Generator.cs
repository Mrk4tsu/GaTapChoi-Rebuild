using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace GaVL.Data.ValueGenerators
{
    public class UuidV7Generator : ValueGenerator<Guid>
    {
        public override bool GeneratesTemporaryValues => false;

        public override Guid Next(EntityEntry entry)
        {
            return UUIDNext.Uuid.NewDatabaseFriendly(UUIDNext.Database.PostgreSql);
        }
    }
}
