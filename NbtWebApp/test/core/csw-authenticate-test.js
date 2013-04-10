/*global Csw:true, QUnit:false, module:false, test:false, asyncTest:false, expect:false*/
/*global start:false, stop:false ok:false, equal:false, notEqual:false, deepEqual:false*/
/*global notDeepEqual:false, strictEqual:false, notStrictEqual:false, raises:false*/

module("Authentication");
asyncTest("Validate AJAX authentication failures", function () {
    window.expect(4);
    var completed = 0;

    $.ajax({
        url: '../Services/Session/Init',
        data: JSON.stringify({ CustomerId: '', UserName: 'admin123', Password: '1' }),
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function(data) {
            completed += 1;
            deepEqual(data.Authentication.AuthenticationStatus, 'NonExistentSession', 'Authentication status is "NonExistentSession" absent a Customer Id');
            if (completed === 4) {
                start();
            }
        }
    });

    $.ajax({
        url: '../Services/Session/Init',
        data: JSON.stringify({ CustomerId: '', UserName: '', Password: '1' }),
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            completed += 1;
            deepEqual(data.Authentication.AuthenticationStatus, 'NonExistentSession', 'Authentication status is "NonExistentSession" absent a Customer Id and Username');
            if (completed === 4) {
                start();
            }
        }
    });
    
    $.ajax({
        url: '../Services/Session/Init',
        data: JSON.stringify({ CustomerId: '', UserName: '', Password: '' }),
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            completed += 1;
            deepEqual(data.Authentication.AuthenticationStatus, 'NonExistentSession', 'Authentication status is "NonExistentSession" absent a Customer Id, Username and Password');
            if (completed === 4) {
                start();
            }
        }
    });
    
    $.ajax({
        url: '../Services/Session/Init',
        data: JSON.stringify({ CustomerId: '1', UserName: '', Password: '' }),
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            completed += 1;
            deepEqual(data.Authentication.AuthenticationStatus, 'NonExistentSession', 'Authentication status is "NonExistentSession" absent a Username and Password');
            if (completed === 4) {
                start();
            }
        }
    });
    
});
    

asyncTest("Validate AJAX authentication success", function () {
    window.expect(2);
    

    $.ajax({
        url: '../Services/Session/Init',
        data: JSON.stringify({ CustomerId: 'nbt_master', UserName: 'admin', Password: 'admin' }),
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            
            deepEqual(data.Authentication.AuthenticationStatus, 'Authenticated', 'Authentication against nbt_master succeeded');

            $.ajax({
                url: '../Services/Session/EndWithAuth',
                data: {},
                type: 'post',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    deepEqual(data.Authentication.AuthenticationStatus, 'Deauthenticated', 'Deauthentication succeeded.');
                    start();
                }
            });
        }
    });

    

    
    
});