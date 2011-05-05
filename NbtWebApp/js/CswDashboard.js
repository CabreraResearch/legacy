; (function ($) {
    $.fn.CswDashboard = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/getDashboard',
			onSuccess: function() { }
        };

        if (options) {
            $.extend(o, options);
        }

        var $DashDiv = $(this);
        
        CswAjaxXml({
            url: o.Url,
            data: "",
            success: function ($xml) {
                 
                    var $table = $DashDiv.CswTable('init', { ID: 'DashboardTable' });
                    $table.addClass('DashboardTable');
                    var $tr = $table.append('<tr />');

                    $xml.children().each(function() {
                        var $this = $(this);

                        var cellcontent = '';
                        if($this.CswAttrXml('href') !== undefined)
                        {
                            cellcontent = '<td class="DashboardCell">' +
                                          '  <a target="_blank" href="'+ $this.CswAttrXml('href') + '">' +
                                          '    <div title="'+ $this.CswAttrXml('text') +'" id="'+ $this.CswAttrXml('id') +'" class="'+ $this.CswAttrXml('id') +'" />' +
                                          '  </a>' +
                                          '</td>';
                        } else {
                            cellcontent = '<td class="DashboardCell">' +
                                          '  <div title="'+ $this.CswAttrXml('text') +'" id="'+ $this.CswAttrXml('id') +'" class="'+ $this.CswAttrXml('id') +'" />' +
                                          '</td>';
                        }
                        $tr.append(cellcontent);

                    });

					o.onSuccess();

            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);




