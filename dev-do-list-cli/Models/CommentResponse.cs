namespace dev_do_list_cli.Models
{
    public class CommentResponse
    {
        public int commentId { get; set; }
        
        public int taskId { get; set; }
        
        public string comment { get; set; }

        public DateTime dateCommented { get; set; }
    }
}
