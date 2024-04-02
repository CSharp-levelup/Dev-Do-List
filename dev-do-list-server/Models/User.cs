using System;
using System.Collections.Generic;

namespace dev_do_list_server.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
}
