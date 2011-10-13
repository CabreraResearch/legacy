/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../controls/CswGrid.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswQuotas';

    var methods = {
        init: function(options) {
			var o = {
				Url: '/NbtWebApp/wsNBT.asmx/getQuotas',
				SaveUrl: '/NbtWebApp/wsNBT.asmx/saveQuotas',
				ID: 'action_quotas'
			};
			if(options) $.extend(o, options);

			var $Div = $(this);
			var $table;
			var row;
			var $cell1, $cell2, $cell3;
			var $savebtn;
			var quotaJson;

			function initTable()
			{
				$Div.contents().remove();
				$table = $Div.CswTable('init', { ID: o.ID + '_tbl', border: 1, cellpadding: 5 });
				row = 1;

				// Header row
				$cell1 = $table.CswTable('cell', row, 1);
				$cell1.append('<b>Class</b>');
				$cell2 = $table.CswTable('cell', row, 2);
				$cell2.append('<b>Includes</b>');
				$cell3 = $table.CswTable('cell', row, 3);
				$cell3.append('<b>Quota</b>');
				row += 1;

				// Quota table
				CswAjaxJson({
					url: o.Url,
					data: {},
					success: function(result) {
						quotaJson = result;
						var canedit = isTrue(quotaJson.canedit);
					
						crawlObject(quotaJson.objectclasses, function (childObj, childKey, parentObj, value) {
							if(tryParseNumber(childObj.nodetypecount) > 0) {

								$cell1 = $table.CswTable('cell', row, 1);
								$cell1.append(childObj.objectclass);

								$cell2 = $table.CswTable('cell', row, 2);
								crawlObject(childObj.nodetypes, function (childObj_nt, childKey_nt, parentObj_nt, value_nt) {
									$cell2.append(childObj_nt.nodetypename + '<br/>');
								}, false);

								$cell3 = $table.CswTable('cell', row, 3);
								if(canedit) {
									$cell3.CswInput({	ID: o.ID + '_' + childObj.objectclassid + '_quota',
														name: o.ID + '_' + childObj.objectclassid + '_quota',
														type: CswInput_Types.text,
														value: childObj.quota,
														width: '50px'
													});
								} else {
									$cell3.append(childObj.quota);
								}

								row += 1;
							}
						}, false); // crawlObject()

						if(canedit) {
							var $savebtn = $Div.CswButton({
								ID: o.ID + '_save',
								enabledText: 'Save',
								disabledText: 'Saving',
								onclick: handleSave
							});
						}
					} // success
				}); // ajax()
			} // initTable()

			function handleSave()
			{
				crawlObject(quotaJson.objectclasses, function (childObj, childKey, parentObj, value) {
					childObj.quota = $('#' + o.ID + '_' + childObj.objectclassid + '_quota').val();
				}, false);

				CswAjaxJson({
					url: o.SaveUrl,
					data: { Quotas: JSON.stringify(quotaJson) },
					success: function(result) {
						initTable();
					}
				});
			} // handleSave()

			initTable();

        } // init
    }; // methods

    
    // Method calling logic
    $.fn.CswQuotas = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
