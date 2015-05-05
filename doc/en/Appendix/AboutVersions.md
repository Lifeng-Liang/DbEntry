About versions
==========

DbEntry already have a lot of versions. Basically the newer version is better than older. And I am not considering the compatibility of versions very much. So DbEntry could keep simple and powerful but not growing to a monster.

The compatibility is not very well between versions,  but the core thinking is same. And almost every version has unit tests and samples, so we can use it to find out how to use the current version.

The versions above(not include) 3.6 is only support .net 3.5, and there is a discussion of how to use vs2008 to develop and deploy it to win2000:  [Why is no .net 2.0 version?](https://dbentry.codeplex.com/discussions/77853).

The versions above(include) 3.5 is based on .Net Framework 3.5. And the core DLLs (Data Util Web) can also works in .Net 2.0 and VS2005.

The versions above (include) 0.20 is based on .Net Framework 2.0.

The versions below (include) 0.19 is based on .Net Framework 1.1.

Version 0.19 also implemented a nullable classes group to fully support null value for database in .Net 1.1.

So if we have a project must use .Net 1.1, for example, SharePoint 2003, the version 0.19 is a good choice.

The older versions were not uploaded to codeplex, it could download from sourceforge: [http://sourceforge.net/projects/dbentry/](http://sourceforge.net/projects/dbentry/)

