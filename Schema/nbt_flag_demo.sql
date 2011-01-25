/* 
   This will need to be updated for future schema.
      1. Join out to ObjectClass instead of relying on NodeTypeName
      2. Currently, all Views, NodeTypes, Properties, Tabs and Welcome content are intended for release. This may change.
   This script only addresses the state of the NBT_MASTER for 01H-17 and below.
*/

update license_accept
set isdemo='1';

update sessionlist
set isdemo='1';

update statistics
set isdemo='1';

update statistics_actions
set isdemo='1';

update statistics_nodetypes
set isdemo='1';

update statistics_reports
set isdemo='1';

update statistics_views
set isdemo='1';

update statistics_searches
set isdemo='1';

update update_history
set isdemo='1';

update nodes
set isdemo='0';

update jct_nodes_props
set isdemo='0';

update nodetypes
set isdemo='0';

update nodetype_props
set isdemo='0';

update schedule_items
set isdemo='0';

update nodetype_tabset
set isdemo='0';

update node_views
set isdemo='0';

update jct_modules_actions
set isdemo='0';

update jct_modules_nodetypes
set isdemo='0';

update sequences
set isdemo='0';

update nodes
set isdemo='1'
where nodetypeid in (select nodetypeid
                     from nodetypes
                     where nodetypename not in ('User','Role') );

update nodes 
set isdemo='1'
where nodetypeid in (select nodetypeid
                     from nodetypes
                     where nodetypename = 'User')
and nodeid not in (select j.nodeid
from jct_nodes_props j
join nodetype_props ntp on j.nodetypepropid=ntp.nodetypepropid
join nodetypes n on ntp.nodetypeid=n.nodetypeid
where n.nodetypename='User'
and ntp.propname = 'Username'
and field1 = 'admin');
                     
update jct_nodes_props
set isdemo='1'
where nodeid in (select nodeid from nodes where isdemo='1');

commit;
