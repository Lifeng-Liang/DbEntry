HtmlBuilder
==========

HtmlBuilder could help us to build html intuitionistic:

````c#
HtmlBuilder hb = HtmlBuilder.New
.table
    .tr
        .td
            .text("test")
        .end
    .end
.end;
string html = hb.ToString();
````

Now, the string html will be:

````html
<table><tr><td>test</td></tr></table>
````

The ``"text"`` will do html encode before it be added, so we don't need to worry about it by ourselves.

And, if we use more or less ``"end"``, it will raise an exception.

If there is nothing between start tag and end tag, it will generate a short closed tag:

````c#
HtmlBuilder hb = HtmlBuilder.New.ul.li.end.end;
````

The result will be:

````html
<ul><li /></ul>
````

To build a hyper link is like following:

````c#
HtmlBuilder.New.a("http://llf.javaeye.com").text("myblog").end.ToString();
````

The generated html snippet is:

````html
<a href="http://llf.javaeye.com">myblog</a>
````

We can include another HtmlBuilder as inner html too:

````c#
HtmlBuilder b = HtmlBuilder.New.a("t.aspx").text("tt").end;
HtmlBuilder b2 = HtmlBuilder.New.li.include(b).end;
string html = b2.ToString();
````

The string html will be:

````html
<li><a href="t.aspx">tt</a></li>
````

Add ``"class"`` to a tag:

````c#
HtmlBuilder b = HtmlBuilder.New.li.Class("nt").text("tt").end;
````

The result will be:

````html
<li class="nt">tt</li>
````

HtmlBuilder only supoort few html tags and attributes. To support more tag and attribute should use ``"tag"`` and ``"attr"`` function:

````c#
HtmlBuilder hb = HtmlBuilder.New
.table
    .tr.attr("width", "100%")
        .td.attr("rowspan", "2")
            .tag("b").Class("red")
            	.text("test")
            .end
        .end
    .end
.end;
````

The result will be:

````html
<table><tr width="100%"><td rowspan="2"><b class="red">test</b></td></tr></table>
````

Have fun!
