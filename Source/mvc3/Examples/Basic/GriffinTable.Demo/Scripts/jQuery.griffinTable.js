/*global window: false */
(function ($) {
	"use strict";
    function toObject(jsonArray) {
        if (!(jsonArray instanceof Array)) {
            return jsonArray;
        }
        var rv = {}, i = 0;
        for (; i < jsonArray.length; ++i) {
            if (jsonArray[i] !== undefined) {
                rv[jsonArray[i].name] = jsonArray[i].value;
            }
        }
        return rv;
    }

    var methods = {
        init: function (options) {
            return this.each(function () {
                var plugin = this;
                var $plugin = $(this);
                var pluginContext = $plugin.data('griffin-table');
                if (typeof pluginContext !== 'undefined') {
                    pluginContext.settings.logger('Already initialized');
                    return $(this); // already initialized
                }

                /** Default settings */
                var settings = { /** Load rows through ajax when table is initialized */
                    fetchAtStart: false,

                    /** Suffix used to find the form which belongs to this table */
                    formSuffix: '-form',

                    /** Suffix used to find the jQuery row template
                    * if the table is named "mytable" then the template should be named
                    * "mytable-template" per default.
                    */
                    rowTemplateSuffix: '-template',

                    /** Specify ID if you have named your form to something special. */
                    formId: null,

                    /** The theme manager to use. */
                    themeManager: $.griffinTableExtensions.themeManagers.jQueryUI,


                    /** styles that should be applied to the table */
                    styles: { /** Class to append to odd rows, used by the default themeManger */
                        oddRowClass: 'odd',

                        /** Class to append to even rows, used by the default themeManager */
                        evenRowClass: ''
                    },

                    /** used to show debug output, default option uses the browser console */
                    logger: function (output) {
                        window.console && console.log && console.log(output);
                    },

                    /** Number of items to load per page. */
                    pageSize: 20,

                    /** Builds the paging at the bottom of a page */
                    pageManager: $.griffinTableExtensions.pageManagers.pageListPager,


                    /** Extension points that you can use to plug into the table */
                    callbacks: {
                        // all rows have been loaded 
                        rowsLoaded: function (addedRowCount, tableRowCount, totalRowCount) { },

                        // will append a row to the table
                        appendingRow: function (rowHtml, rowJson) { },

                        // about to get rows through ajax
                        fetchingRows: function (frm) { },

                        // have fetched the rows but not processed them yet
                        fetchedRows: function (frm, data) { },

                        formatColumn: function (rowHtml, rowJson, columnName, columnValue) {
                            return columnValue;
                        }
                    },

                    formatters: {
                        currency: function (value) {
                            return value;
                        }


                    }

                };
                if (options) {
                    $.extend(settings, options);
                }
                settings.name = $(this).attr('id');
                if (settings.name === '') {
                    alert('All griffin tables must have an ID.');
                    return false;
                }

                // pluginContext which will be stored as data
                pluginContext = {
                    settings: settings,
                    $table: $(this),
                    logger: function (msg) {
                        this.settings.logger(msg);
                    },
                    plugin: this
                };

                if (settings.formId === null) {
                    pluginContext.form = $('#' + settings.name + settings.formSuffix);
                    if (pluginContext.form.length === 0) {
                        alert('All griffin tables must have a corrensponding form. Missing a form named "' + settings.name + settings.formSuffix + '".');
                        return false;
                    }
                }
                else {
                    pluginContext.form = $('#' + settings.formId);
                    if (pluginContext.form.length === 0) {
                        alert('All griffin tables must have a corrensponding form. Missing a form named "' + settings.formId + '".');
                        return false;
                    }
                }

                // manual search = no paging.
                $('input[type=submit]', pluginContext.form).click(function () {
                    plugin.resetPaging();
                });

                pluginContext.form.submit(function (evt) {
                    evt.preventDefault();

                    var formData = pluginContext.form.serialize();
                    var url = pluginContext.form.attr('action');

                    var pos = pluginContext.$table.position();
                    var $overlay = $('<div style="background-color: grey;opacity: 0.5;">Loading</div>');
                    $('body').append($overlay);
                    $overlay.css({
                        position: 'absolute',
                        top: pos.top,
                        left: pos.left,
                        width: pluginContext.$table.width() + "px",
                        height: pluginContext.$table.height() + "px"
                    });
                    $.get(url, formData, function (json) {
                        plugin.loadData(json);
                        $overlay.remove();
                    });

                    return false;
                });

                pluginContext.rowRendering = {
                    plugin: $plugin
                };
                var templateNode = $('#' + settings.name + settings.rowTemplateSuffix);
                if (templateNode.length === 1) {
                    $.template("rowTemplate", templateNode); //outerhtml: .clone().wrap('<div></div>').parent().html()
                    pluginContext.rowRendering.render = function (row) {
                        return pluginContext.plugin.renderRowUsingTemplate(row);
                    };
                }
                else {
                    pluginContext.rowRendering.render = function (row) {
                        return pluginContext.plugin.renderRow(row);
                    };
                }

                /** Go through thead and load all columns into our own column array.
                * Inserts column names as input elements  in the form.
                * Adds sorting inputs in the form
                */
                this.initializeColumns = function () {
                    var index = 0;
                    pluginContext.columns = [];
                    pluginContext.columnNameMapping = [];

                    $('thead tr th', $plugin).each(function () {
                        var column = {
                            element: $(this),
                            index: index,
                            name: $(this).attr('rel')
                        };

                        column.hidden = column.element.css('display') === 'none' || column.element.hasClass('hidden');
                        pluginContext.columns[index] = column;
                        pluginContext.columnNameMapping[column.name] = column;
                        column.element.data('griffinColumn', column);
                        column.element.click(function () {
                            pluginContext.plugin.headerClick(column, pluginContext);
                        });

                        if (column.name.toLowerCase() === 'id') {
                            $plugin.data('griffin-id-column', column);
                        }

                        pluginContext.form.append('<input type="hidden" name="Column" value="' + column.name + '" />');
                        ++index;
                    });

                    pluginContext.form.append('<input type="hidden" name="SortColumn" value="" />');
                    pluginContext.form.append('<input type="hidden" name="SortOrder" value="" />');
                    $plugin.find('thead th').css('cursor', 'pointer');
                };

                this.initializePaging = function () {
                    pluginContext.settings.pageManager.init($plugin, pluginContext.form, settings.themeManager);
                    pluginContext.form.append('<input type="hidden" name="PageNumber" value="1" />');
                    pluginContext.form.append('<input type="hidden" name="PageSize" value="20" />');
                    if (pluginContext.settings.totalRowCount > 0) {
                        pluginContext.settings.pageManager.loadingRows(pluginContext.$table, plugin.getCurrentPage(), options.totalRowCount, { canClear: false });
                        pluginContext.settings.pageManager.rowsLoaded(pluginContext.$table, plugin.getCurrentPage(), options.totalRowCount);
                    }
                };

                this.resetPaging = function () {
                    $('input[name=PageNumber]', pluginContext.form).val('1');
                };

                this.getCurrentPage = function () {
                    return parseInt($('input[name=PageNumber]', pluginContext.form).val(), 10);
                };

                // Takes an (json) object and renders it using a template
                this.renderRowUsingTemplate = function (row) {
                    if (typeof row !== 'object') {
                        row = toObject(row);
                    }

                    return $.tmpl("rowTemplate", row);
                };

                // takes an array (assumes that the array items is in the correct order)
                this.renderRow = function (row) {

                    var fetchColumnValue = function (row, index) {
                        return row[index];
                    };
                    if (!(row instanceof Array)) {
                        fetchColumnValue = function (row, index) {
                            return row[pluginContext.columns[index].name];
                        };
                    }

                    var $row = $('<tr></tr>');
                    $.each(pluginContext.columns, function (columnIndex, column) {
                        var $cell = $('<td></td>');
                        if (column.hidden) {
                            $cell.css('display', 'none');
						}
                        $cell.html(fetchColumnValue(row, columnIndex));
                        $cell.appendTo($row);
                    });

                    return $row;
                };


                this.headerClick = function (column, pluginContext) {
                    var $this = $(column.element);

                    var currentSort = '';
                    if ($this.hasClass('asc')) {
                        currentSort = 'asc';
                    }
                    if ($this.hasClass('desc')) {
                        currentSort = 'desc';
                    }

                    $('thead tr th', pluginContext.plugin).removeClass('asc').removeClass('desc');
                    switch (currentSort) {
                        case 'asc':
                            currentSort = 'desc';
                            break;
                        case 'desc':
                            currentSort = '';
                            break;
                        default:
                            currentSort = 'asc';
                            break;
                    }

                    if (currentSort !== '') {
                        $this.addClass(currentSort);
                    }

                    pluginContext.settings.themeManager.clearSorting(pluginContext.plugin);
                    pluginContext.settings.themeManager.applySorting($this, currentSort);

                    $('input[name=SortOrder], pluginContent.form').val(currentSort);
                    $('input[name=SortColumn], pluginContent.form').val($this.attr('rel'));
                    
                    // reset paging on sort.
                    $('input[name=PageNumber], pluginContent.form').val('1');

                    pluginContext.plugin.submitForm();

                };

                this.submitForm = function () {
                    pluginContext.form.submit();
                };

                this.getData = function () {

                };

                this.loadData = function (data) {
                    var rowCount = $plugin.children('tbody tr').length;

                    if (typeof data.Rows === 'undefined') {
                        data.Rows = data;
                        data.TotalRowCount = 0;
                    }

                    var $tbody = $('tbody', $plugin);


                    pluginContext.settings.pageManager.loadingRows(pluginContext.$table, plugin.getCurrentPage(), data.TotalRowCount, { canClear: true });
                    $.each(data.Rows, function (rowIndex, row) {

                        var renderedRow = pluginContext.rowRendering.render(row);
                        pluginContext.settings.themeManager.applyRowTheme(renderedRow, rowCount);

                        renderedRow.appendTo($tbody);
                        ++rowCount;
                    });
                    pluginContext.settings.pageManager.rowsLoaded(pluginContext.$table, plugin.getCurrentPage(), data.TotalRowCount);
                };




                // rest of initialization here.
                this.initializeColumns();
                this.initializePaging();
                settings.themeManager.applyTheme($plugin);

                $plugin.data('griffin-table', pluginContext);
                if (settings.fetchAtStart) {
                    pluginContext.form.submit();
                }

                return $(this);
            });
        },
        destroy: function () {

            return this.each(function () {

                var $this = $(this),
                    data = $this.data('tooltip');

                // Namespacing FTW
                $(window).unbind('.tooltip');
                data.tooltip.remove();
                $this.removeData('tooltip');

            });

        },

        // *****************
        // Load new data into the table where each row is an column array
        //
        // Each row will be transformed into a row array. 
        // ************************
        loadData: function (data) {
            if (data === undefined) {
                return this;
            }
            if (!(data instanceof Array)) {
                alert(typeof data);
                alert('loadData expects to get an array.');
                return this;
            }

            var pluginContext = $(this).data('griffin-table');
            pluginContext.plugin.loadData(data);

            return $(this);
        },

        show: function () { // ... 
        },
        hide: function () { // ... 
        },
        update: function (content) { // ...
        }
    };



    $.fn.griffinTable = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.griffinTable');
        }

        return this;



    };

})(jQuery);

$.griffinTableExtensions = {
    pageManagers: {},
    themeManagers: {}
};

$.griffinTableExtensions.pageManagers.showMoreLinkPager = {
    init: function ($table, $form, themeManager) {
        $moreLink = $('<a style="display:none" href="" id=' + $table.attr('id') + '-pager' + '>Show more</a>');
        var settings = {
            $table: $table,
            themeManager: themeManager,
            $form: $form,
            $moreLink: $moreLink
        };


        themeManager.applyButtonStyle($moreLink);

        $moreLink.click(function (evt) {
            evt.preventDefault();
            var newValue = parseInt($('input[name=PageNumber]', settings.$form).val(), 10) + 1;
            $('input[name=PageNumber]', settings.$form).val(newValue + '');
            settings.$form.submit();
        });


        $table.data('pager-settings', settings);
        $table.after($moreLink);
    },


    rowsLoaded: function ($table, currentPageNumber, totalRows) {
        var settings = $table.data('pager-settings');
        var rowsAdded = $('tbody tr', settings.$table).length;

        if (rowsAdded < totalRows) {
            settings.$moreLink.show();
        } else {
            settings.$moreLink.hide();

        }
    }
};
$.griffinTableExtensions.pageManagers.pageListPager = {


    init: function ($table, $form, themeManager) {
        $container = $('<div class="griffin-table-pager" style="text-align:right;width:100%;"></div>');
        var settings = {
            $table: $table,
            themeManager: themeManager,
            $form: $form,
            $container: $container
        };


        $table.data('pager-settings', settings);
        $table.after($container);
    },


    loadingRows: function ($table, currentPageNumber, totalRows, options) {
        if (options.canClear) {
            $('tbody tr', $table).remove();
		}
    },


    rowsLoaded: function ($table, currentPageNumber, totalRows) {
        var settings = $table.data('pager-settings');
        var rowsAdded = $('tbody tr', settings.$table).length;

        if (rowsAdded < totalRows) {
            if (currentPageNumber <= 1) {
                var pageSize = parseInt($('input[name=PageSize]', settings.$form).val(), 10);
                var rest = totalRows % pageSize;
                var pageCount = (totalRows - rest) / pageSize;
                if (rest > 0) {
                    pageCount++;
				}
				
                $('a', settings.$container).remove();
                var pageNumber;
                for (pageNumber = 1; pageNumber <= pageCount; ++pageNumber) {
                    var exec = function (myNumber) {
                        var $pageLink = $('<a id="' + $table.attr('id') + '_page_' + myNumber + '" href="#"> ' + pageNumber + '</a>');
                        settings.themeManager.applyButtonStyle($pageLink);
                        $pageLink.appendTo(settings.$container);
                        settings.$container.append('&nbsp;');
                        $pageLink.click(function () {
                            $('input[name=PageNumber]', settings.$form).val(myNumber);
                            settings.$form.submit();
                        });
                    };


                    exec(pageNumber); //to get number in scope

                }
            }

            $('a.active', settings.$container).removeClass('active');
            if (currentPageNumber === 0) {
                currentPageNumber = 1;
			}
			
            var id = $table.attr('id') + '_page_' + currentPageNumber;
            $('#' + id).addClass('active');
            settings.themeManager.removeActiveButtonStyle($('a', settings.$container));
            settings.themeManager.applyActiveButtonStyle($('#' + id));

        } else {
            settings.$container.hide();

        }
    }
};


// Apply jQuery UI theme to the table
$.griffinTableExtensions.themeManagers.jQueryUI = {

    applyTheme: function ($table) {
        $('thead th', $table).each(function (row) {
            var $this = $(this);
            var contents = $this.html();
            $this.addClass('ui-state-default');

            // no sorting for this column
            if (contents === '' || contents === ' ' || contents === '&nbsp;') {
                return this;
            }



            contents = '<span class="contents">' + contents + '</span><div class="ui-icon ui-icon-triangle-2-n-s" style="float:right;vertical-align:middle"></div>';
            $this.html(contents);

            return this;
        });




        $table.addClass('ui-widget');
        $table.attr('cellspacing', '0');
        $table.attr('cellpadding', '0');
        $table.css('border-collapse', 'collapse');
        $table.css('width', '100%');
    },


    applyButtonStyle: function ($elem) {
        $elem.addClass('ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only');
        $elem.css('margin-top', '10px');
        $elem.css('padding', '10px');
    },


    applyRowTheme: function ($tr, rowNumber) {
        //if (rowNumber % 2 === 1) {
        //  $tr.children('td').addClass('ui-widget-content');
        //}
    },



    applyActiveButtonStyle: function ($elem) {
        $elem.addClass('ui-state-active');
    },


    removeActiveButtonStyle: function ($elem) {
        $elem.removeClass('ui-state-active');
    },


    clearSorting: function ($table) {
        $('thead th .ui-icon', $table).removeClass('ui-icon-triangle-1-s');
        $('thead th .ui-icon', $table).removeClass('ui-icon-triangle-1-n');
        $('thead th .ui-icon', $table).addClass('ui-icon-triangle-2-n-s');
        $('thead th', $table).removeClass('ui-state-highlight');
    },


    // order = 'asc', 'desc' or ''.
    applySorting: function ($th, order) {
        $($th).addClass('ui-state-highlight');
        switch (order) {
            case 'asc':
                $('.ui-icon', $th).removeClass('ui-icon-triangle-2-n-s');
                $('.ui-icon', $th).addClass('ui-icon-triangle-1-n');
                break;

            case 'desc':
                $('.ui-icon', $th).removeClass('ui-icon-triangle-2-n-s');
                $('.ui-icon', $th).addClass('ui-icon-triangle-1-s');
                break;

            default:
                $th.removeClass('ui-state-highlight');
                break;


        }
    }
};

// use no theme
$.griffinTableExtensions.themeManagers.noTheme = {
    /**
    * Apply the theme to the table.
    * @param $table Table that should get a theme applied
    */
    applyTheme: function ($table) { },




    /**
     * Apply theme to a link or button
     */
    applyButtonStyle: function ($elem) {
    },




    /**
     * Highlight the active/selected button
     */
    applyActiveButtonStyle: function ($elem) {
    },






     /** Remove active state */
    removeActiveButtonStyle: function ($elem) {
    },







    /**
    * Apply theme to a newly created row.

    * @param $tr Table row (as a jQuery object)
    * @param rowNumber zero-based index
    */
    applyRowTheme: function ($tr, rowNumber) { },







    /**
    * Remove all previously configured sorting icons etc.
    * Invoked when a header has been clicked (before applySorting())
    * @param $table Target table (as a jQuery object)
    */
    clearSorting: function ($table) { },









    /**
    * Apply sorting icons etc to the clicked header
    * @param $th Clicked row as jQuery object
    * @param order Can be 'asc', 'desc' or ''. Empty string = no sorting
    */
    applySorting: function ($th, order) {
    }
};



