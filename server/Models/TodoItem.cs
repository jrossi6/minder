using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minder.Server.Models
{
    public class TodoItem
    {
        private const string DEFAULT_NAME = "New Todo Item";
        private DateTime? dueOn;
        private IList<long> childItems;

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Key{ get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set;}
        public DateTime? DueOn
        {
            get
            {
                return dueOn;
            }
            set
            {
                dueOn = (value?.Kind == DateTimeKind.Utc) ? value : value?.ToUniversalTime();
            }
        }
        public DateTime CreatedOn { get; }
        public long? ParentKey { get; set; }

        public IEnumerable<long> ChildItems
        {
            get
            {
                return this.childItems;
            }
        }

        public int CompleteChildren { get; private set;}
        public TodoItem()
        {
            this.CreatedOn = DateTime.UtcNow;
            this.childItems = new List<long>();
        }

        public void AddChildItem(TodoItem child)
        {
            if (child == null)
            {
                throw new ArgumentNullException();
            }
            child.ParentKey = this.Key;
            this.childItems.Add(child.Key);
        }

        public void RemoveChildItem(TodoItem child)
        {
            if (child == null)
            {
                throw new ArgumentNullException();
            }
            this.childItems.Remove(child.Key);
        }

        public void OnChildIsCompleteChange(bool newValue)
        {
            this.CompleteChildren += (newValue) ? 1 : -1;
        }

        public void CopyFrom(TodoItem item)
        {
            this.Name = item.Name;
            this.IsComplete = item.IsComplete;
            this.DueOn = item.DueOn;
            this.ParentKey = item.ParentKey;
        }
    }
}