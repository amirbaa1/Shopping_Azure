using Microsoft.AspNetCore.Mvc;
using Steeltoe.Management.Info;

namespace ClientServiceEureka.Controllers;

public class MyInfoContributor : IInfoContributor
{
    public void Contribute(IInfoBuilder builder)
    {
        builder.WithInfo("MyInfo", new
        {
            Name = "API TEST INFO",
            Description = "تست اطلاعات در euerka",
            email = "a@b.com",
        });
    }
}