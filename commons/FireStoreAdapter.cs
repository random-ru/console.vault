using System;
using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace vault;


public class FireStoreAdapter : IStoreAdapter
{
    private FirestoreDb db { get; }
    public FireStoreAdapter() => db = 
        FirestoreDb.Create(Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID"));
    public CollectionReference Namespaces 
        => db.Collection("namespaces");
    
    public IAsyncEnumerable<CollectionReference> Apps(string space) 
        => db
            .Collection("namespaces")
            .Document(space)
            .ListCollectionsAsync();

    public DocumentReference Keys(string space, string app) 
        => db
            .Collection("namespaces")
            .Document(space)
            .Collection(app)
            .Document("values");
}

public interface IStoreAdapter
{
    CollectionReference Namespaces { get; }
    IAsyncEnumerable<CollectionReference> Apps(string space);
    DocumentReference Keys(string space, string app);
}