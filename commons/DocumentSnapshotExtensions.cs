using Google.Cloud.Firestore;

namespace vault.commons;


public static class DocumentSnapshotExtensions
{
    public static T? FetchValue<T>(this DocumentSnapshot snapshot, string key)
    {
        if (!snapshot.Exists) return default;
        if (!snapshot.ContainsField(key)) return default;
        return snapshot.GetValue<T>(key);
    }
}