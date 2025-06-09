using System;

public abstract class BaseApi {
    //protected static readonly string BaseUrl = "http://localhost:3000/api";
    protected static readonly string BaseUrl = "https://farmplease-server.onrender.com/api";
}

[Serializable]
public class AbstractMongoEntity {
    public string _id;
    public string createdAt;
    public string updatedAt;
    public string __v;

    public DateTime CreatedAt => DateTime.Parse(createdAt);
    public DateTime UpdatedAt => DateTime.Parse(updatedAt);
}