/*
    Cookie: .AspNetCore.Cookies=CfDJ8NqQwVH9wspFt2nkQ4EBMjMbSsR7Wi4LujLDEGWzDQCAK0Yj5xpaixm-2FpgrhaxSZAXSrTfAxFv9mZzWiraVBcGreAZ--DW5sJQ7Lt0MBn7A2OLuje7h-KLT1ONxBHwOC8S1ocarDr8FzJ-srbsmgsfirK3zKfEOWvjPippgw4q1ltx7DLEJMhCC1Ijt3IPgtQj-h8zcWzXTZwYob56Yw6oSDhpVDniX46ja8Nd_KRbcYyffBxrTSIoTtBbVEJMWkOwAM-_itPa9Ah_wkgQHNWS6iHNZY8-GEqnsNLUQNCdW_CAdNVqsAiaClA_O44-WBIOhXfajcKPhiO9lry8C-25efse_Uzf8fTPgZZR-doDKmgo2djXuxcMG_I5mLKGZGxKnmlzJih40Vx714k9S5fRuQL2mBNYLmotvq6QQ9H3fBKExI-HZXVk_4ywEFbcPxmDZW8bxYLJLSr23UwCgdPeTGmO0Qhv6nBOLPTxcTmFznpZzpaGQ67BLYw1urbmxNpn2raES1tCaaprM66Bb55Fo8x4skLOmRMksb05qfZZrFP58gV8H08xMM3qONQSvYr-vG8MywBrZQuZb85QdnSQ1QP5bbnpRuD6Cc8qX8qM66xtBMiJdAAXdfT16VmLJa_YFiCKjnAg0hhhiAnGYoK-rqH8D67yJHPGy3SM9I0ordSOx6N9AoOdlAj0xcCKAL1stnpONO573qwT6mVflEMAVqhvleDTcn8eWIaWseRkTJdVRFo6mtylEfqETeAOvI_wo3xnDKCKIObOYcNEe8PQ7UDpWddhfvOo9UuO_6j20mg49XX7h64O1pF1EgCLaBJvrJ2shz2KiXF6KPsztXsmfuYh54ZobP7KHsRjay8atOsoW8IaqsxEhBRR8OthK7CihtXco2zYGTlik9HJcgrvXGiWjJKlmiCJw9y2XambHGCUfG1T77N-BQav5bSUhQ

    GET: http://localhost:5012/api/Staff/search?name=Laura%20Múrias&email=tomasandreleite@gmail.com    
*/

pm.test("Status code is 200", function () {
    pm.expect(pm.response.code).to.equal(200);
});

pm.test("Response time is less than 2 seconds", function () {
    pm.expect(pm.response.responseTime).to.be.below(2000);
});

pm.test("All values in the first response item match the expected values", function () {
    const jsonData = pm.response.json();
    const firstItem = jsonData[0];

    pm.expect(firstItem.id).to.equal("T202400001");
    pm.expect(firstItem.licenseNumber).to.equal("SN567842");
    pm.expect(firstItem.name).to.equal("Laura Múrias");
    pm.expect(firstItem.email).to.equal("tomasandreleite@gmail.com");
    pm.expect(firstItem.phone).to.equal("555-12342");
    pm.expect(firstItem.address).to.equal("123 Main St, Sample City, Sample State");
    
    const expectedAvailabilitySlots = [
        "21-11-2024T09:00:00 - 21-11-2024T11:00:00",
        "22-11-2024T13:00:00 - 22-11-2024T21:00:00"
    ];
    pm.expect(JSON.stringify(firstItem.availabilitySlots)).to.equal(JSON.stringify(expectedAvailabilitySlots));
    
    pm.expect(firstItem.specialization).to.equal("Cardiology");
});
