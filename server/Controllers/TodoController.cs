using Microsoft.AspNetCore.Mvc;
using Minder.Server.Models;
using System;
using System.Collections.Generic;

namespace server.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly ITodoRepository todoRepository;

        public TodoController(ITodoRepository todoRepository)
        {
            this.todoRepository = todoRepository;
        }
        // GET api/todo
        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return todoRepository.GetAll();
        }

        // GET api/todo/{id}
        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(long id)
        {
            TodoItem item = todoRepository.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST api/todo
        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            todoRepository.Add(item);

            this.UpdateParent(item, (p, c) => p.AddChildItem(c));

            return CreatedAtRoute("GetTodo", new { id = item.Key }, item);
        }

        // PUT api/todo/{id}
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoItem item)
        {
            if (item == null || item.Key != id)
            {
                return BadRequest();
            }

            TodoItem todo = todoRepository.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            bool parentChanged = todo.ParentKey != item.ParentKey;

            if (parentChanged)
            {
                UpdateParent(todo, (p, c) => p.RemoveChildItem(c));
            }

            todo.CopyFrom(item);

            if (parentChanged)
            {
                UpdateParent(todo, (p, c) => p.AddChildItem(c));
            }

            todoRepository.Update(todo);
            return new NoContentResult();
        }

        // DELETE api/todo/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            TodoItem todo = todoRepository.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            todoRepository.Remove(id);
            return new NoContentResult();
        }

        private void UpdateParent(TodoItem item, Action<TodoItem, TodoItem> operation)
        {
            if (item.ParentKey != null)
            {
                TodoItem parentItem = todoRepository.Find( (long)item.ParentKey);
                operation(parentItem, item);
                todoRepository.Update(parentItem);
            }
        }
    }
}
