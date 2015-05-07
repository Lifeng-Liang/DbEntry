Paged Query
==========

Paged query is a common requirement in most database based application. DbEntry provide an interface to do this easily.

There is a class named ``PagedSelector``, we can use it to select the page we want. And it uses ``Range`` clause in DbEntry, ``Range`` clause uses the most matched method to invoke database by its dialect.

For example, it will use ``top`` clause and skip the front items to get the page we want if we use SqlServer 2000.

It will use ``limit`` clause to get the page we want if we use SQLite or MySql.

It will use ``Row_Number`` to get the page we want if we use SqlServer 2005.

And the code in DbEntry are totally same.

We can use PagedSelector.GetCurrentPage to get the list of the page we want, PagedSelector also provides GetResultCount and GetPageCount to help us for paging.

````c#
var ps = DbEntry
    .From<SampleData>()
    .Where(Condition.Empty)
    .OrderBy((DESC)"Id")
    .PageSize(pageSize)
    .GetPagedSelector();
var count = ps.GetResultCount();
var list = ps.GetCurrentPage(pageIndex);
````

Just call GetDistinctPagedSelector to get distinct paged selector.

Above code shows how to use code to do paging, DbEntry also supports DbEntryDataSource to do paging more easily, TRY IT.

Static paged selector
----------

We already have ``PagedSelector``, why we need ``StaticPagedSelector``?

Normally, we use the ``Id DESC`` as order clause to show the items, and paged them too. So we will get the dynamic results when we get the page by page number if the new items added. It's ok for search function, but if we use it for news list or something like that, the dynamic result will perturb the search engine, and normally, the most requests are start from search engine.

To solve this issue, we can use ``Id`` as order clause to get the page, and it will works well with the database which supported paged select like ``limit`` clause, but if we change it to SqlServer 2000, it will use ``Top`` clause, and the most recent page will be the most slowly query of it.

So, we have another paged selector called ``StaticPagedSelector``, it implements the same interface as ``PagedSelector``. If we want something to friendly to search engine, we can use it.

The ``From`` sytax support static paged selector too, so we get it like this:

````c#
var ps = DbEntry
    .From<SampleData>()
    .Where(Condition.Empty)
    .OrderBy((DESC)"Id")
    .PageSize(10)
    .GetStaticPagedSelector();
````

Just call GetDistinctStaticPagedSelector to get distinct static paged selector.
