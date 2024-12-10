using System;
using System.Collections.Generic;

namespace TodoApi;

public partial class User
{
    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Token { get; set; }

    public string Email { get; set; } = null!;
}
