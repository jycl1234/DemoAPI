# DemoAPI

This is a sample API endpoint application written in C# for demonstration purposes.

## Methods

The base URL is at `/api`. This will show a list of all records.

`/api/getitem/{id}`: This returns an item by `id`. Will return `success: false` on not found or errors.

`/api/getmaxprices`: This returns a list of items, sorted by name, then sorted by cost in descending order. Only the highest cost of each item is shown.

`/api/getmaxprice/{itemname}`: This returns the max price of an item by `itemname`, which should be a string identical to an item name in the table.

`/api/update`: This updates an existing item. The payload must be in JSON format, with Id (number), ItemName (string), and Cost (number).

All endpoints return `success: true` on successful query, and `success: false` on not found or errors.
