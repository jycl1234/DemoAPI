# DemoAPI

This is a sample API endpoint application written in C# for demonstration purposes.

## Methods

The base URL is at `/api`. This will show a list of all records in JSON format.

`/api/getitem/{id}`: This returns an item by `id`, which should be an integer. Will return null if item is not found.

`/api/getmaxprices`: This returns a list of items, sorted by name, then sorted by cost in descending order. Only the highest cost of each item is shown.

`/api/getmaxprice/{id}`: This returns the max price of an item by `id`, which should be an integer. Will return `0` if item is not found.