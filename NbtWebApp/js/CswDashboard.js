; (function ($) {
    $.fn.CswDashboard = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/getDashboard'
        };

        if (options) {
            $.extend(o, options);
        }

        var $DashDiv = $(this);
        
        CswAjaxXml({
            url: o.Url,
            data: "",
            success: function ($xml) {
                 
                    //var $table = $('<table id="DashboardTable" class="DashboardTable" cellpadding="0" cellspacing="0"><tr></tr></table>');
                    var $table = makeTable('DashboardTable');
                    $table.addClass('DashboardTable');
                    var $tr = $table.append('<tr />');

                    $xml.children().each(function() {
                        var $this = $(this);

                        var cellcontent = '';
                        if($this.attr('href') != undefined)
                        {
                            cellcontent = '<td class="DashboardCell">' +
                                          '  <a target="_blank" href="'+ $this.attr('href') + '">' +
                                          '    <div title="'+ $this.attr('text') +'" id="'+ $this.attr('id') +'" class="'+ $this.attr('id') +'" />' +
                                          '  </a>' +
                                          '</td>';
                        } else {
                            cellcontent = '<td class="DashboardCell">' +
                                          '  <div title="'+ $this.attr('text') +'" id="'+ $this.attr('id') +'" class="'+ $this.attr('id') +'" />' +
                                          '</td>';
                        }
                        $tr.append(cellcontent);

                        $DashDiv.text('')
                                .append($table);
                    });

            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);




