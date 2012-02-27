# v1.0.5

## Implemented all callbacks

https://github.com/jgauffin/Griffin.Table/wiki/Callbacks

## Storing JSON object on each row:

Each `<tr>` tag that is loaded through the plugin gets it's data/json stored using $().data() which means that you can use jQuery to load it at any time.

Example

    alert($('#myTable tbody tr:first').data('griffin-table-data').FirstName + ' is the name of the first user');
    
## The odd class for TR styling is now configurable

    $('#myTable').griffinTable({ style: { oddRowClass: 'myOdd' }});
    
## Added plain html/javascript examples

https://github.com/jgauffin/Griffin.Table/tree/master/Source/js/Examples
