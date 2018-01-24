# Movie-WebAPI
WebAPI demo with memory caching and dependency injection

This is an example of a WebAPI that uses a third party DLL as a data source. Calling the third party data source is costly and data only gets updated every 24 hour. So the WebAPI uses the Memory Caching to avoid calling third party every time that gets the data. There is also a front-end which uses AngularJS to consume the WebAPI to display, search, sort, insert and update the data. For unit testing, it uses Moq to test WebAPI methods.
