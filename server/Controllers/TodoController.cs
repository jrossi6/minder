using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minder.Server.Models;

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

            if (todo.ParentKey != null)
            {
                TodoItem parentItem = todoRepository.Find( () item)
            }

            todo.CopyFrom(item);

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
