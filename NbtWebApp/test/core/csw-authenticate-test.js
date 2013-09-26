/*global Csw:true, QUnit:false, module:false, test:false, asyncTest:false, expect:false*/
/*global start:false, stop:false ok:false, equal:false, notEqual:false, deepEqual:false*/
/*global notDeepEqual:false, strictEqual:false, notStrictEqual:false, raises:false*/

module("Authentication");
asyncTest("Validate AJAX authentication failure - no info", function () {
	$.ajax({
        url: '../Services/Session/Init',
        data: JSON.stringify({ CustomerId: '', UserName: '', Password: '' }),
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            deepEqual(data.Authentication.AuthenticationStatus, 'NonExistentSession', 'Authentication status is "NonExistentSession" absent a Customer Id, Username and Password');
            start();
        },
		error: function (error) {
			var err = error;//For Debugging
		}
    });
});

asyncTest("Validate AJAX authentication failure - no Customer Id", function () {
	$.ajax({
        url: '../Services/Session/Init',
        data: JSON.stringify({ CustomerId: '', UserName: 'admin123', Password: '1' }),
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            deepEqual(data.Authentication.AuthenticationStatus, 'NonExistentSession', 'Authentication status is "NonExistentSession" absent a Customer Id');
            start();
        },
		error: function (error) {
			var err = error;//For Debugging
		}
    });
});

asyncTest("Validate AJAX authentication failure - no Customer Id or Username", function () {
	$.ajax({
        url: '../Services/Session/Init',
        data: JSON.stringify({ CustomerId: '', UserName: '', Password: '1' }),
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            deepEqual(data.Authentication.AuthenticationStatus, 'NonExistentSession', 'Authentication status is "NonExistentSession" absent a Customer Id and Username');
            start();
        },
		error: function (error) {
			var err = error;//For Debugging
		}
    });
});

asyncTest("Validate AJAX authentication failure - no Username or Password", function () {
	$.ajax({
        url: '../Services/Session/Init',
        data: JSON.stringify({ CustomerId: '1', UserName: '', Password: '' }),
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            deepEqual(data.Authentication.AuthenticationStatus, 'NonExistentSession', 'Authentication status is "NonExistentSession" absent a Username and Password');
            start();
        },
		error: function (error) {
			var err = error;//For Debugging
		}
    });
});

//Commenting out for now - we shouldn't be hardcoding accessids anyway
/*asyncTest("Validate AJAX authentication success", function () {
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
});*/