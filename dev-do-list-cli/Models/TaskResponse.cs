namespace dev_do_list_cli.Models
{
    public  class TaskResponse
    {
        public int taskId { get; set; }

        public string title { get; set; }

        public string? description { get; set; }

        public DateTime dateCreated { get; set; }

        public DateTime? dueDate { get; set; }

        public int userId { get; set; }

        public int statusId { get; set; }

        public int taskTypeId { get; set; }

        public ICollection<CommentResponse> comments { get; set; }
    }
}
