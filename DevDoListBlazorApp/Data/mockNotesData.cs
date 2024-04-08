using DevDoListBlazorApp.Models;

namespace DevDoListBlazorApp.Data
{
    public class MockNotesData
    {
        public List<Note> GenerateMockNotes()
        {
            var mockNotes = new List<Note>
            {
                new Note
                {
                    noteId = 1,
                    title = "Meeting Agenda",
                    description = "Prepare agenda for the weekly team meeting.",
                    dueDate = DateTime.Now.AddDays(3), // Example due date 3 days from now
                    userId = 1,
                    statusId = 1,
                    taskTypeId = 1,
                    comments = new Comment[]
                    {
                        new Comment { commentId = 1, comment = "Discuss project updates.", dateCommented = DateTime.Now },
                        new Comment { commentId = 2, comment = "Review action items from the previous meeting.", dateCommented = DateTime.Now }
                    }
                },
                new Note
                {
                    noteId = 2,
                    title = "Bug Fix",
                    description = "Fix issue #123 reported by user.",
                    dueDate = DateTime.Now.AddDays(5), // Example due date 5 days from now
                    userId = 2,
                    statusId = 2,
                    taskTypeId = 1,
                    comments = new Comment[]
                    {
                        new Comment { commentId = 3, comment = "Investigate root cause of the bug.", dateCommented = DateTime.Now },
                        new Comment { commentId = 4, comment = "Implement and test the fix.", dateCommented = DateTime.Now }
                    }
                },
            };

            return mockNotes;
        }
    }
}

