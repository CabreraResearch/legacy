;(function ($) {
/**
 * jqGrid Persian Translation
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
	$.jgrid = {
        defaults: {
            recordtext: "Ù†Ù…Ø§Ø¨Ø´ {0} - {1} Ø§Ø² {2}",
            emptyrecords: "Ø±Ú©ÙˆØ±Ø¯ÛŒ ÛŒØ§ÙØª Ù†Ø´Ø¯",
            loadtext: "Ø¨Ø§Ø±Ú¯Ø²Ø§Ø±ÙŠ...",
            pgtext: "ØµÙØ­Ù‡ {0} Ø§Ø² {1}"
        },
        search: {
            caption: "Ø¬Ø³ØªØ¬Ùˆ...",
            Find: "ÙŠØ§ÙØªÙ‡ Ù‡Ø§",
            Reset: "Ø§Ø² Ù†Ùˆ",
            odata: ['Ø¨Ø±Ø§Ø¨Ø±', 'Ù†Ø§ Ø¨Ø±Ø§Ø¨Ø±', 'Ø¨Ù‡', 'Ú©ÙˆÚ†Ú©ØªØ±', 'Ø§Ø²', 'Ø¨Ø²Ø±Ú¯ØªØ±', 'Ø´Ø±ÙˆØ¹ Ø¨Ø§', 'Ø´Ø±ÙˆØ¹ Ù†Ø´ÙˆØ¯ Ø¨Ø§', 'Ù†Ø¨Ø§Ø´Ø¯', 'Ø¹Ø¶Ùˆ Ø§ÛŒÙ† Ù†Ø¨Ø§Ø´Ø¯', 'Ø§ØªÙ…Ø§Ù… Ø¨Ø§', 'ØªÙ…Ø§Ù… Ù†Ø´ÙˆØ¯ Ø¨Ø§', 'Ø­Ø§ÙˆÛŒ', 'Ù†Ø¨Ø§Ø´Ø¯ Ø­Ø§ÙˆÛŒ'],
            groupOps: [{
                op: "AND",
                text: "Ú©Ù„"
            },
            {
                op: "OR",
                text: "Ù…Ø¬Ù…ÙˆØ¹"
            }],
            matchText: " Ø­Ø§ÙˆÛŒ",
            rulesText: " Ø§Ø·Ù„Ø§Ø¹Ø§Øª"
        },
        edit: {
            addCaption: "Ø§Ø¶Ø§ÙÙ‡ Ú©Ø±Ø¯Ù† Ø±Ú©ÙˆØ±Ø¯",
            editCaption: "ÙˆÙŠØ±Ø§ÙŠØ´ Ø±Ú©ÙˆØ±Ø¯",
            bSubmit: "Ø«Ø¨Øª",
            bCancel: "Ø§Ù†ØµØ±Ø§Ù",
            bClose: "Ø¨Ø³ØªÙ†",
            saveData: "Ø¯ÛŒØªØ§ ØªØ¹ÛŒÛŒØ± Ú©Ø±Ø¯! Ø°Ø®ÛŒØ±Ù‡ Ø´ÙˆØ¯ØŸ",
            bYes: "Ø¨Ù„Ù‡",
            bNo: "Ø®ÛŒØ±",
            bExit: "Ø§Ù†ØµØ±Ø§Ù",
            msg: {
                required: "ÙÙŠÙ„Ø¯Ù‡Ø§ Ø¨Ø§ÙŠØ¯ Ø®ØªÙ…Ø§ Ù¾Ø± Ø´ÙˆÙ†Ø¯",
                number: "Ù„Ø·ÙØ§ Ø¹Ø¯Ø¯ ÙˆØ¹ØªØ¨Ø± ÙˆØ§Ø±Ø¯ Ú©Ù†ÙŠØ¯",
                minValue: "Ù…Ù‚Ø¯Ø§Ø± ÙˆØ§Ø±Ø¯ Ø´Ø¯Ù‡ Ø¨Ø§ÙŠØ¯ Ø¨Ø²Ø±Ú¯ØªØ± ÙŠØ§ Ù…Ø³Ø§ÙˆÙŠ Ø¨Ø§",
                maxValue: "Ù…Ù‚Ø¯Ø§Ø± ÙˆØ§Ø±Ø¯ Ø´Ø¯Ù‡ Ø¨Ø§ÙŠØ¯ Ú©ÙˆÚ†Ú©ØªØ± ÙŠØ§ Ù…Ø³Ø§ÙˆÙŠ",
                email: "Ù¾Ø³Øª Ø§Ù„Ú©ØªØ±ÙˆÙ†ÙŠÚ© ÙˆØ§Ø±Ø¯ Ø´Ø¯Ù‡ Ù…Ø¹ØªØ¨Ø± Ù†ÙŠØ³Øª",
                integer: "Ù„Ø·ÙØ§ ÙŠÚ© Ø¹Ø¯Ø¯ ØµØ­ÙŠØ­ ÙˆØ§Ø±Ø¯ Ú©Ù†ÙŠØ¯",
                date: "Ù„Ø·ÙØ§ ÙŠÚ© ØªØ§Ø±ÙŠØ® Ù…Ø¹ØªØ¨Ø± ÙˆØ§Ø±Ø¯ Ú©Ù†ÙŠØ¯",
                url: "Ø§ÛŒÙ† Ø¢Ø¯Ø±Ø³ ØµØ­ÛŒØ­ Ù†Ù…ÛŒ Ø¨Ø§Ø´Ø¯. Ù¾ÛŒØ´ÙˆÙ†Ø¯ Ù†ÛŒØ§Ø² Ø§Ø³Øª ('http://' ÛŒØ§ 'https://')",
                nodefined: " ØªØ¹Ø±ÛŒÙ Ù†Ø´Ø¯Ù‡!",
                novalue: " Ù…Ù‚Ø¯Ø§Ø± Ø¨Ø±Ú¯Ø´ØªÛŒ Ø§Ø¬Ø¨Ø§Ø±ÛŒ Ø§Ø³Øª!",
                customarray: "ØªØ§Ø¨Ø¹ Ø´Ù…Ø§ Ø¨Ø§ÛŒØ¯ Ù…Ù‚Ø¯Ø§Ø± Ø¢Ø±Ø§ÛŒÙ‡ Ø¯Ø§Ø´ØªÙ‡ Ø¨Ø§Ø´Ø¯!",
                customfcheck: "Ø¨Ø±Ø§ÛŒ Ø¯Ø§Ø´ØªÙ† Ù…ØªØ¯ Ø¯Ù„Ø®ÙˆØ§Ù‡ Ø´Ù…Ø§ Ø¨Ø§ÛŒØ¯ Ø³Ø·ÙˆÙ† Ø¨Ø§ Ú†Ú©ÛŒÙ†Ú¯ Ø¯Ù„Ø®ÙˆØ§Ù‡ Ø¯Ø§Ø´ØªÙ‡ Ø¨Ø§Ø´ÛŒØ¯!"
            }
        },
        view: {
            caption: "Ù†Ù…Ø§ÛŒØ´ Ø±Ú©ÙˆØ±Ø¯",
            bClose: "Ø¨Ø³ØªÙ†"
        },
        del: {
            caption: "Ø­Ø°Ù",
            msg: "Ø§Ø² Ø­Ø°Ù Ú¯Ø²ÙŠÙ†Ù‡ Ù‡Ø§ÙŠ Ø§Ù†ØªØ®Ø§Ø¨ Ø´Ø¯Ù‡ Ù…Ø·Ù…Ø¦Ù† Ù‡Ø³ØªÙŠØ¯ØŸ",
            bSubmit: "Ø­Ø°Ù",
            bCancel: "Ø§Ø¨Ø·Ø§Ù„"
        },
        nav: {
            edittext: " ",
            edittitle: "ÙˆÙŠØ±Ø§ÙŠØ´ Ø±Ø¯ÙŠÙ Ù‡Ø§ÙŠ Ø§Ù†ØªØ®Ø§Ø¨ Ø´Ø¯Ù‡",
            addtext: " ",
            addtitle: "Ø§ÙØ²ÙˆØ¯Ù† Ø±Ø¯ÙŠÙ Ø¬Ø¯ÙŠØ¯",
            deltext: " ",
            deltitle: "Ø­Ø°Ù Ø±Ø¯Ø¨Ù Ù‡Ø§ÙŠ Ø§Ù†ØªØ®Ø§Ø¨ Ø´Ø¯Ù‡",
            searchtext: " ",
            searchtitle: "Ø¬Ø³ØªØ¬ÙˆÙŠ Ø±Ø¯ÙŠÙ",
            refreshtext: "",
            refreshtitle: "Ø¨Ø§Ø²ÙŠØ§Ø¨ÙŠ Ù…Ø¬Ø¯Ø¯ ØµÙØ­Ù‡",
            alertcap: "Ø§Ø®Ø·Ø§Ø±",
            alerttext: "Ù„Ø·ÙØ§ ÙŠÚ© Ø±Ø¯ÙŠÙ Ø§Ù†ØªØ®Ø§Ø¨ Ú©Ù†ÙŠØ¯",
            viewtext: "",
            viewtitle: "Ù†Ù…Ø§ÛŒØ´ Ø±Ú©ÙˆØ±Ø¯ Ù‡Ø§ÛŒ Ø§Ù†ØªØ®Ø§Ø¨ Ø´Ø¯Ù‡"
        },
        col: {
            caption: "Ù†Ù…Ø§ÙŠØ´/Ø¹Ø¯Ù… Ù†Ù…Ø§ÙŠØ´ Ø³ØªÙˆÙ†",
            bSubmit: "Ø«Ø¨Øª",
            bCancel: "Ø§Ù†ØµØ±Ø§Ù"
        },
        errors: {
            errcap: "Ø®Ø·Ø§",
            nourl: "Ù‡ÙŠÚ† Ø¢Ø¯Ø±Ø³ÙŠ ØªÙ†Ø¸ÙŠÙ… Ù†Ø´Ø¯Ù‡ Ø§Ø³Øª",
            norecords: "Ù‡ÙŠÚ† Ø±Ú©ÙˆØ±Ø¯ÙŠ Ø¨Ø±Ø§ÙŠ Ù¾Ø±Ø¯Ø§Ø²Ø´ Ù…ÙˆØ¬ÙˆØ¯ Ù†ÙŠØ³Øª",
            model: "Ø·ÙˆÙ„ Ù†Ø§Ù… Ø³ØªÙˆÙ† Ù‡Ø§ Ù…Ø­Ø§Ù„Ù Ø³ØªÙˆÙ† Ù‡Ø§ÙŠ Ù…Ø¯Ù„ Ù…ÙŠ Ø¨Ø§Ø´Ø¯!"
        },
        formatter: {
            integer: {
                thousandsSeparator: " ",
                defaultValue: "0"
            },
            number: {
                decimalSeparator: ".",
                thousandsSeparator: " ",
                decimalPlaces: 2,
                defaultValue: "0.00"
            },
            currency: {
                decimalSeparator: ".",
                thousandsSeparator: " ",
                decimalPlaces: 2,
                prefix: "",
                suffix: "",
                defaultValue: "0"
            },
            date: {
                dayNames: ["ÙŠÚ©", "Ø¯Ùˆ", "Ø³Ù‡", "Ú†Ù‡Ø§Ø±", "Ù¾Ù†Ø¬", "Ø¬Ù…Ø¹", "Ø´Ù†Ø¨", "ÙŠÚ©Ø´Ù†Ø¨Ù‡", "Ø¯ÙˆØ´Ù†Ø¨Ù‡", "Ø³Ù‡ Ø´Ù†Ø¨Ù‡", "Ú†Ù‡Ø§Ø±Ø´Ù†Ø¨Ù‡", "Ù¾Ù†Ø¬Ø´Ù†Ø¨Ù‡", "Ø¬Ù…Ø¹Ù‡", "Ø´Ù†Ø¨Ù‡"],
                monthNames: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Ú˜Ø§Ù†ÙˆÙŠÙ‡", "ÙÙˆØ±ÙŠÙ‡", "Ù…Ø§Ø±Ø³", "Ø¢ÙˆØ±ÙŠÙ„", "Ù…Ù‡", "Ú˜ÙˆØ¦Ù†", "Ú˜ÙˆØ¦ÙŠÙ‡", "Ø§ÙˆØª", "Ø³Ù¾ØªØ§Ù…Ø¨Ø±", "Ø§Ú©ØªØ¨Ø±", "Ù†ÙˆØ§Ù…Ø¨Ø±", "December"],
                AmPm: ["Ø¨.Ø¸", "Ø¨.Ø¸", "Ù‚.Ø¸", "Ù‚.Ø¸"],
                S: function (b) {
                    return b < 11 || b > 13 ? ["st", "nd", "rd", "th"][Math.min((b - 1) % 10, 3)] : "th"
                },
                srcformat: "Y-m-d",
                newformat: "d/m/Y",
                masks: {
                    ISO8601Long: "Y-m-d H:i:s",
                    ISO8601Short: "Y-m-d",
                    ShortDate: "n/j/Y",
                    LongDate: "l, F d, Y",
                    FullDateTime: "l, F d, Y g:i:s A",
                    MonthDay: "F d",
                    ShortTime: "g:i A",
                    LongTime: "g:i:s A",
                    SortableDateTime: "Y-m-d\\TH:i:s",
                    UniversalSortableDateTime: "Y-m-d H:i:sO",
                    YearMonth: "F, Y"
                },
                reformatAfterEdit: false
            },
            baseLinkUrl: "",
            showAction: "Ù†Ù…Ø§ÙŠØ´",
            target: "",
            checkbox: {
                disabled: true
            },
            idName: "id"
        }
    }
})(jQuery);