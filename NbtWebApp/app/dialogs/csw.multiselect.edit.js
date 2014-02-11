/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('multiselectedit', function (cswPrivate) {
        'use strict';

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.title = cswPrivate.title || 'Edit Property';
            cswPrivate.onSave = cswPrivate.onSave || function () { };
            cswPrivate.required = cswPrivate.required || false;
            cswPrivate.inDialog = cswPrivate.inDialog || false;
            cswPrivate.parent = cswPrivate.parent; //for when we don't render in a dialog,
            cswPrivate.height = cswPrivate.height || '400px';
            cswPrivate.width = cswPrivate.width || '750px';
            cswPrivate.onChange = cswPrivate.onChange || function () { };
            cswPrivate.usePaging = cswPrivate.usePaging || true;
            cswPrivate.itemsPerPage = cswPrivate.itemsPerPage || 100;
            cswPrivate.currentPage = 1;
            cswPrivate.visibleOptions = [];
            cswPrivate.filter = '';

            //javascript does not have a concept of a deep copy, so we must perform one manually so that 
            //we can add data for rendering each option without messing up our webservices
            //Also note that cswPrivate.opts is the value actually passed in when calling multiselectedit 
            cswPrivate.options = [];
            cswPrivate.opts.forEach(function(option) {
                cswPrivate.options.push({ 'value': option.value, 'selected': option.selected, 'text': option.text });
            });
        }());
        
        function calculateTotalPages() {
            //determine whether we should calculate based on the filtered set of results
            var activeList;
            if (Csw.isNullOrEmpty(cswPrivate.filter)) {
                activeList = cswPrivate.options;
            } else {
                activeList = cswPrivate.filteredOptions;
            }
            //get page count by taking items in the list over items per page, and always round up (1 item on a new page is still a full page)
            var total = Math.ceil(activeList.length / cswPrivate.itemsPerPage);
            return total;
        }//calculateTotalPages()


        //construct the span that describes current and total pages
        cswPrivate.pageInfo = function (parent) {
            //clear out the old div
            parent.empty();
            //fetch the number of pages, and create an array of the page numbers from 1 to N
            var numberOfPages = calculateTotalPages();
            var options = new Array(numberOfPages);
            for (var i = 0; i < numberOfPages; i++) { options[i] = i + 1; }
            
            //create the component
            var pageCountContainer = parent.span();
            pageCountContainer.span({ text: 'Page ' });
            var pageChoices = pageCountContainer.select({
                values: options,
                selected: cswPrivate.currentPage,
                onChange: function() {
                    //when a new page number is clicked, jump to that page and re-render options
                    cswPrivate.currentPage = parseInt(pageChoices.val());
                    updateVisibleOptions();
                }
            });
            pageCountContainer.span({ text: ' of ' + numberOfPages });
            
            return pageCountContainer;
        };//pageInfo()


        cswPrivate.getSelectedOptions = function () {
            //iterate through the list, adding each selected item's value to the list to return
            var selected = [];
            cswPrivate.options.forEach(function(option) {
                if (Csw.bool(option.selected)) {
                    selected.push(option.value);
                }
            });
            return selected;
        };

        //return an array of the selected item values
        cswPublic.val = function () {
            return cswPrivate.getSelectedOptions();
        };


        //construct the static components of the composite
        cswPrivate.make = function () {

            var makeCtrl = function (multiSelectDiv) {
                cswPrivate.ctrlOpts = {};
                var filterInput = multiSelectDiv.input({
                    labelText: 'Filter:',
                    placeholder: 'Enter keywords',
                    onKeyUp: function () {
                        var arrayToFilter;
                        if (filterInput.val().toLowerCase().contains(cswPrivate.filter) && false == Csw.isNullOrEmpty(cswPrivate.filter)) {
                            //user added letters to an existing filter, so we only need to refine from previously filtered options
                            arrayToFilter = cswPrivate.filteredOptions.splice(0);
                        } else {
                            //otherwise, we need to go through the whole list
                            arrayToFilter = cswPrivate.options;
                        }
                        cswPrivate.filteredOptions = [];
                        
                        //after text is entered in the filter box, store the filter
                        cswPrivate.filter = filterInput.val().toLowerCase();
                        //then recalculate the filtered list of options if the filter is not empty
                        if (false == Csw.isNullOrEmpty(cswPrivate.filter)) {
                            //for each item in the list
                            Csw.iterate(arrayToFilter, function (item) {
                                //if the item matches the filter, store a reference to it in the filtered options array
                                if (item.text.toLowerCase().contains(filterInput.val().toLowerCase())) {
                                    cswPrivate.filteredOptions.push(item);
                                } //if the item matches the filter
                            }); //iterate through the full list of results
                        }//if the filter is not blank
                        
                        //always go back to the first page when someone has changed the search
                        cswPrivate.currentPage = 1;
                        updateVisibleOptions();
                        
                        //re-render the page list, because total number of pages may have changed
                        cswPrivate.pageInfo(cswPrivate.pageDisplay);
                    }//after a new letter is entered in the filter
                });//filter input

                //create a table to store the check/uncheck buttons
                var btnTbl = multiSelectDiv.table({
                    cellpadding: 5
                });
                btnTbl.cell(1, 1).div().buttonExt({
                    enabledText: 'Uncheck All',
                    onClick: function () {
                        //iterate through all the currently visible things, and uncheck them
                        Csw.iterate(cswPrivate.visibleOptions, function (opt) {
                            onCheck(opt, false);
                        });
                    }
                });//uncheck all button
                btnTbl.cell(1, 2).div().buttonExt({
                    enabledText: 'Check All',
                    onClick: function () {
                        //iterate through all the currently visible things, and check them
                        Csw.iterate(cswPrivate.visibleOptions, function (opt) {
                            onCheck(opt, true);
                        });
                    }
                });//check all button

                errorDiv = multiSelectDiv.div().span({ text: 'At least one value must be selected' }).css('color', 'red');
                errorDiv.hide();

                optsDiv = multiSelectDiv.div();
                optsDiv.css({ 'height': cswPrivate.height, 'width': cswPrivate.width, 'overflow': 'auto', 'border': '1px solid #AED0EA' });

                cswPrivate.optionsTbl = optsDiv.table().css('padding', '10px');

                if (cswPrivate.usePaging) {
                    //if we are paging, we must create a strip of components along the bottom of the multiselect to manage page changes
                    pagingDiv = multiSelectDiv.div();
                    cswPrivate.pagingTbl = pagingDiv.table().css('padding', '10px');

                    cswPrivate.pagingTbl.cell(1, 1).buttonExt({
                        enabledText: 'Previous',
                        onClick: onPrevious,
                    });
                    //put a little space between the previous and next buttons
                    cswPrivate.pagingTbl.cell(1, 2).span({
                        text: '&nbsp;&nbsp;&nbsp;'
                    });

                    cswPrivate.pagingTbl.cell(1, 3).buttonExt({
                        enabledText: 'Next',
                        onClick: onNext,
                    });

                    cswPrivate.pagingTbl.cell(1, 3).span({
                        text: '&nbsp;&nbsp;&nbsp;'
                    });
                    //display a "Page X of Y" message after the buttons
                    cswPrivate.pageDisplay = cswPrivate.pagingTbl.cell(1, 5);
                    cswPrivate.pageInfo(cswPrivate.pageDisplay);
                }//if (cswPrivate.usePaging)

            };//makeCtrl()

            var saveBtnClicked = false;
            var optsDiv, errorDiv, pagingDiv;

            if (cswPrivate.inDialog) {
                //if this multiselect is popping up in its own dialog, we need to create the dialog
                var editDialog = Csw.layouts.dialog({
                    title: cswPrivate.title,
                    width: 800,
                    height: 600,
                    onOpen: function () {
                        //create all the components we just defined inside the dialog
                        makeCtrl(editDialog.div);

                        editDialog.div.buttonExt({
                            enabledText: 'Save Changes',
                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                            onClick: function () {
                                errorDiv.hide();
                                saveBtnClicked = true;
                                Csw.clientChanges.unsetChanged(); //closing a csw.dialog fires manual validation, which we don't want here
                                var isValid = Csw.tryExec(cswPrivate.onSave, cswPrivate.getSelectedOptions());
                                if (isValid || false === cswPrivate.required) {
                                    editDialog.close();
                                } else {
                                    errorDiv.show();
                                }
                            }
                        }).css('margin-top', '20px');

                    }//onOpen()
                });//edit dialog
                editDialog.open();
            } else {
                //when not in a dialog, just draw the components to the page
                makeCtrl(cswPrivate.parent);
            }
        };//cswPrivate.make()


        //when the previous page button is clicked
        function onPrevious() {
                var currentPage = cswPrivate.currentPage;
                var previousPage = cswPrivate.currentPage - 1;

                //if we're not already on the first page
                if (currentPage != 1) {
                    //set the page, then re-render the visible options accordingly
                    cswPrivate.currentPage = previousPage;
                    updateVisibleOptions();
                    //update the page text at the bottom of the page
                    cswPrivate.pageInfo(cswPrivate.pageDisplay);
                }//if we're not already on the first page
        }//onPrevious()


        //when the next page button is clicked
        function onNext() {
            var nextPage = cswPrivate.currentPage + 1;

            //if we're not already on the last page
            if (false === (nextPage > calculateTotalPages())) {
                //set the page, then re-render the visible options accordingly
                cswPrivate.currentPage = nextPage;
                updateVisibleOptions();
                //update the page text at the bottom of the page
                cswPrivate.pageInfo(cswPrivate.pageDisplay);
            }//if we're not already on the last page
        }//onNext()



        //when an option is checked
        function onCheck(option, isChecked) {
            //set the checkbox to the right value in the UI
            option.ctrl.checkbox.checked(isChecked);
            //set the option to the right value in the data model
            option.selected = isChecked;
            //register that the user has been modifying this component (for warnings before page reload)
            Csw.clientChanges.setChanged();
            //fire a change event attached to this component
            cswPrivate.onChange(cswPrivate.getSelectedOptions());
        };//onCheck()

        function updateVisibleOptions() {
            //tear down all the old options. In an ideal world we'll figure out a way to reuse divs, but the algorithm in place before this screwed up sorting after filters.
            Csw.iterate(cswPrivate.visibleOptions, function(option) {
                option.ctrl.ctrlDiv.empty();
                option.ctrl = null;
            });
            cswPrivate.visibleOptions = [];
            
            //determine whether we should use the filtered results or the full list
            var optionsList = null;
            if (false == Csw.isNullOrEmpty(cswPrivate.filter)) {
                optionsList = cswPrivate.filteredOptions;
            } else {
                optionsList = cswPrivate.options;
            }

            //for all of the options on the current page
            for (var i = (cswPrivate.currentPage-1) * cswPrivate.itemsPerPage; (i < cswPrivate.currentPage * cswPrivate.itemsPerPage && i < optionsList.length) ; i++) {
                //I have no clue why this closure is necessary, but without it toggling checkboxes only affect the last element on the page
                (function () {
                //grab the option from the array we are using, and calculate its position
                var currentOption = optionsList[i];
                var position = i - (cswPrivate.currentPage-1)*cswPrivate.itemsPerPage +1;

                    //construct a new checkbox and text blurb for this option
                    currentOption.ctrl = {};
                    currentOption.ctrl.ctrlDiv = cswPrivate.optionsTbl.cell(position, 1).div();
                    currentOption.ctrl.checkbox = currentOption.ctrl.ctrlDiv.input({
                        name: 'chkbx' + position,
                        type: Csw.enums.inputTypes.checkbox,
                        canCheck: true,
                        checked: Csw.bool(currentOption.selected),
                        onChange: function() {
                            onCheck(currentOption, currentOption.ctrl.checkbox.checked() );
                        }
                    });
                    currentOption.ctrl.ctrlDiv.span({ text: currentOption.text, value: currentOption.text });

                    //add this item to the array of visible options, for the benefit of check/uncheck all
                    cswPrivate.visibleOptions.push(currentOption);
                })();
            }//for all options on the current page
        }//updateVisibleOptions()


        (function _postCtor() {
            cswPrivate.make();
            updateVisibleOptions();
        }());

        return cswPublic;
    });
}());