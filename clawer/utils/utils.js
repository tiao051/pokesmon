const axios = require('axios');

async function downloadImageAsBase64(url) {
    if (!url) return null;

    try {
        const response = await axios({
            url,
            method: 'GET',
            responseType: 'arraybuffer'
        });
        
        const base64 = Buffer.from(response.data, 'binary').toString('base64');
        const mimeType = response.headers['content-type'] || 'image/jpeg';
        
        return `data:${mimeType};base64,${base64}`;
    } catch (err) {
        console.error(`Error downloading image ${url}:`, err.message);
        return null;
    }
}

function getSearchPayload(from, size) {
    return {
        "algorithm": "sales_dismax",
        "from": from,
        "size": size,
        "filters": {
            "term": {},
            "range": {},
            "match": {}
        },
        "listingSearch": {
            "context": {
                "cart": {
                    "packages": {}
                }
            },
            "filters": {
                "term": {
                    "sellerStatus": "Live",
                    "channelId": 0
                },
                "range": {
                    "quantity": {
                        "gte": 1
                    }
                },
                "exclude": {
                    "channelExclusion": 0
                }
            }
        },
        "context": {
            "cart": {
                "packages": {}
            },
            "shippingCountry": "VN",
            "userProfile": {}
        },
        "settings": {
            "useFuzzySearch": true,
            "didYouMean": {}
        },
        "sort": {}
    };
}

function getSearchHeaders() {
    return {
        "accept": "application/json, text/plain, */*",
        "accept-language": "vi,vi-VN;q=0.9,en;q=0.8",
        "cache-control": "no-cache",
        "content-type": "application/json",
        "pragma": "no-cache",
        "priority": "u=1, i",
        "sec-ch-ua": "\"Google Chrome\";v=\"147\", \"Not.A/Brand\";v=\"8\", \"Chromium\";v=\"147\"",
        "sec-ch-ua-mobile": "?0",
        "sec-ch-ua-platform": "\"Windows\"",
        "sec-fetch-dest": "empty",
        "sec-fetch-mode": "cors",
        "sec-fetch-site": "same-site",
        "cookie": "tracking-preferences={%22version%22:1%2C%22destinations%22:{%22Actions%20Amplitude%22:true%2C%22AdWords%22:true%2C%22Braze%20Cloud%20Mode%20(Actions)%22:true%2C%22Google%20AdWords%20New%22:true%2C%22Google%20Enhanced%20Conversions%22:true%2C%22Google%20Tag%20Manager%22:true%2C%22Impact%20Partnership%20Cloud%22:true}%2C%22custom%22:{%22advertising%22:true%2C%22functional%22:true%2C%22marketingAndAnalytics%22:true}}; product-display-settings=sort=price+shipping&size=10; setting=CD%3DVN%26M%3D1; TCG_VisitorKey=574a5bcd-74d0-4790-a079-db51c00e4812; international_shipping_modal_seen=true; SellerProximity=ZipCode=&MaxSellerDistance=1000&IsActive=false; SearchSortSettings=M=1&ProductSortOption=BestMatch&ProductSortDesc=False&PriceSortOption=Shipping&ProductResultDisplay=grid; tcg-segment-session=1777679684494%257C1777682425725",
        "Referer": "https://www.tcgplayer.com/"
    };
}

module.exports = {
    downloadImageAsBase64,
    getSearchPayload,
    getSearchHeaders
};
