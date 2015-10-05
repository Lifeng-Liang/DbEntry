Scaffolding
==========

Basically we can define controller like following:

````c#
public class BookController : ControllerBase
{
}
````

If we want DbEntry to build the default code and views of all CRUD operation, we can just inherits from the generic type ``ControllerBase<T>``:

````c#
public class BookController : ControllerBase<Book>
{
}
````

It means we already have the list/new/edit/destroy views of Book model. And we can run application to test it for insert some items, list them etc.

If we want define our own action, we can just override it:

````c#
public class BookController : ControllerBase<Book>
{
    public override void List(long? PageIndex, int? PageSize)
    {
        // your code
    }
}
````

The key/value should insert to the bag is following:

* List
 * list
 * list_count
 * list_pagesize
* Show
 * item
* Edit
 * item

The key/value should insert to the flash is following:

* Create
 * notice
* Update
 * notice

Most time, we/customer do not very sure about the final production at the beginning of the project. By use scaffolding, we can build prototype quickly, and it is real run able application so we/customer can use it to find is it match our purpose or not.
