namespace dev_do_list_cli.Models
{
    public class CommentCreate
    {
        public int taskId { get; set; }

        public string comment { get; set; }

        public DateTime dateCommented { get; set; }
    }
}
