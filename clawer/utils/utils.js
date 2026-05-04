const axios = require('axios');
const COOKIE = require('../constants/cookie');

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
        "cookie": COOKIE,
        "Referer": "https://www.tcgplayer.com/"
    };
}

module.exports = {
    downloadImageAsBase64,
    getSearchPayload,
    getSearchHeaders
};
