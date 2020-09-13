namespace AntiHarassment.Core
{
    public interface ISecurity
    {
        bool TryCreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }
}
