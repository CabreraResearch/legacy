/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.4-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswFieldTypeNFPA';

    var methods = {
        init: function(o) {

            var $Div = $(this);
            var propVals = o.propData.values;
			var red = (false === o.Multi) ? propVals.flammability : CswMultiEditDefaultValue;
			var yellow = (false === o.Multi) ? propVals.reactivity : CswMultiEditDefaultValue;
			var blue = (false === o.Multi) ? propVals.health : CswMultiEditDefaultValue;
			var white = (false === o.Multi) ? propVals.special : CswMultiEditDefaultValue;

			var $outertable = $Div.CswTable('init', { ID: o.ID + '_tbl' });

			var $table = $outertable.CswTable('cell', 1, 1)
										.CswTable('init', { ID: o.ID + '_tbl' })
										.addClass('CswFieldTypeNFPA_table');

			var $cell11 = $table.CswTable('cell', 1, 1)
							.addClass('CswFieldTypeNFPA_red CswFieldTypeNFPA_cell');
			var $reddiv = $('<div class="CswFieldTypeNFPA_text"></div>')
							.appendTo($cell11);

			var $cell12 = $table.CswTable('cell', 1, 2)
							.addClass('CswFieldTypeNFPA_yellow CswFieldTypeNFPA_cell');
			var $yellowdiv = $('<div class="CswFieldTypeNFPA_text"></div>')
							.appendTo($cell12);

			var $cell21 = $table.CswTable('cell', 2, 1)
							.addClass('CswFieldTypeNFPA_blue CswFieldTypeNFPA_cell');
			var $bluediv = $('<div class="CswFieldTypeNFPA_text"></div>')
							.appendTo($cell21);

			var $cell22 = $table.CswTable('cell', 2, 2)
							.addClass('CswFieldTypeNFPA_white CswFieldTypeNFPA_cell');
			var $whitediv = $('<div class="CswFieldTypeNFPA_text"></div>')
							.appendTo($cell22);

			function setValue($div, value)
			{
				$div.text(value);

				if(value === "W")
					$div.addClass("strikethrough");
				else
					$div.removeClass("strikethrough");
			}

			setValue($reddiv, red);
			setValue($yellowdiv, yellow);
			setValue($bluediv, blue);
			setValue($whitediv, white);

            if(!o.ReadOnly)
            {
				var $edittable = $outertable.CswTable('cell', 1, 2)
												.CswTable('init', { ID: o.ID + '_edittbl',
																	FirstCellRightAlign: true });
                var selVals = [
                    { value: '0', display: '0' },
                    { value: '1', display: '1' },
                    { value: '2', display: '2' },
                    { value: '3', display: '3' },
                    { value: '4', display: '4' }
                ];
                if (o.Multi) {
                    selVals.push({ value: CswMultiEditDefaultValue, display: CswMultiEditDefaultValue });
                }
				function makeSelect($cell, id, selected, $div)
				{
					var $sel = $cell.CswSelect({
											'ID': makeId({ ID: o.ID, suffix: id }),
											'selected': selected,
											'values': selVals,
											'cssclass': '',
											'onChange': function () {
												setValue($div, $sel.val());
											}
										});
				} // makeSelect()

				$edittable.CswTable('cell', 1, 1).append('Flammability');
				$edittable.CswTable('cell', 2, 1).append('Health');
				$edittable.CswTable('cell', 3, 1).append('Reactivity');
				$edittable.CswTable('cell', 4, 1).append('Special');

				makeSelect($edittable.CswTable('cell', 1, 2), 'red', red, $reddiv);
				makeSelect($edittable.CswTable('cell', 2, 2), 'yellow', yellow, $yellowdiv);
				makeSelect($edittable.CswTable('cell', 3, 2), 'blue', blue, $bluediv);

                var whiteVals = [
                    { value: 'ACID', display: 'ACID' },
                    { value: 'ALK', display: 'ALK' },
                    { value: 'BIO', display: 'BIO' },
                    { value: 'COR', display: 'COR' },
                    { value: 'CRYO', display: 'CRYO' },
                    { value: 'CYL', display: 'CYL' },
                    { value: 'OX', display: 'OX' },
                    { value: 'POI', display: 'POI' },
                    { value: 'RAD', display: 'RAD' },
                    { value: 'W', display: 'W' }];
                if (o.Multi) {
                    whiteVals.push({ value: CswMultiEditDefaultValue, display: CswMultiEditDefaultValue });
                }
				var $whitesel = $edittable.CswTable('cell', 4, 2)
    				                      .CswSelect({
										     'ID': makeId({ ID: o.ID, suffix: 'white' }),
										     'selected': white,
										     'values': whiteVals,
										     'cssclass': '',
										     'onChange': function () {
											    setValue($whitediv, $whitesel.val());
										     }
									      });

            } // if(!o.ReadOnly)
        },
        save: function(o) {
            var attributes = {
                flammability: null,
                reactivity: null,
                health: null,
                special: null
            };
            var $red = o.$propdiv.find('#' + o.ID + '_red');
            if (false === isNullOrEmpty($red)) {
                attributes.flammability = $red.val();
            }
            var $yellow = o.$propdiv.find('#' + o.ID + '_yellow');
            if (false === isNullOrEmpty($yellow)) {
                attributes.reactivity = $yellow.val();
            }
            var $blue = o.$propdiv.find('#' + o.ID + '_blue');
            if (false === isNullOrEmpty($blue)) {
                attributes.health = $blue.val();
            }
            var $white = o.$propdiv.find('#' + o.ID + '_white');
            if (false === isNullOrEmpty($white)) {
                attributes.special = $white.val();
            }
            preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeNFPA = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
