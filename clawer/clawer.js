require('dotenv').config();
const mongoose = require('mongoose');
const connectDB = require('./utils/connectDB');
const axios = require('axios');
const Item = require('./models/Item');
const { SEARCH_URL, DETAIL_URL, IMAGE_URL } = require('./constants/endpoint');

async function downloadImageAsBase64(url) {
    if (!url) return null;

    try {
        const response = await axios({
            url,
            method: 'GET',
            responseType: 'arraybuffer' // Fetch as buffer
        });
        
        // Convert buffer to Base64
        const base64 = Buffer.from(response.data, 'binary').toString('base64');
        const mimeType = response.headers['content-type'] || 'image/jpeg';
        
        return `data:${mimeType};base64,${base64}`;
    } catch (err) {
        console.error(`Error downloading image ${url}:`, err.message);
        return null;
    }
}

async function scrapeData() {
    console.log(`Starting to scrape data...`);
    
    let from = 0;
    const size = 24;
    let hasNextPage = true;
    
    while (hasNextPage) {
        console.log(`Fetching from offset: ${from}`);
        
        try {
            const response = await axios.post(SEARCH_URL, {
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
            }, {
                headers: {
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
                }
            });
            
            const resultsData = response.data.results[0];
            const items = resultsData.results;
            const totalResults = resultsData.totalResults;
            
            console.log(`Found ${items.length} items. Total results available: ${totalResults}`);
            
            if (items.length === 0) {
                hasNextPage = false;
                break;
            }
            
            for (let item of items) {
                const productId = item.productId;
                console.log(`Processing product ID: ${productId}`);
                
                try {
                    const existingItem = await Item.findOne({ productId: productId });
                    if (existingItem) {
                        console.log(`Product ID ${productId} already exists in DB. Skipping...`);
                        continue;
                    }
                    const detailUrl = DETAIL_URL.replace('[PRODUCT_ID]', productId);
                    const detailResponse = await axios.get(detailUrl);
                    const detailData = detailResponse.data;
                    
                    const imageUrl = IMAGE_URL.replace('${PRODUCT_ID}', productId);
                    const imageBase64 = await downloadImageAsBase64(imageUrl);
                    
                    const itemDataToSave = {
                        ...item,
                        ...detailData,
                        imageUrl,
                        imageBase64
                    };
                    
                    await Item.findOneAndUpdate(
                        { productId: productId },
                        itemDataToSave,
                        { upsert: true, returnDocument: 'after' }
                    );
                    console.log(`Saved product ID: ${productId}`);
                } catch (err) {
                    console.error(`Failed to process product ID: ${productId}`, err.message);
                }
            }
            
            from += size;
            
            if (from >= totalResults) {
                hasNextPage = false;
            }
            
        } catch (error) {
            console.error(`Error fetching search page at offset ${from}:`, error.message);
            hasNextPage = false;
        }
    }
    
    console.log(`Finished scraping data.`);
}

async function run() {
    await connectDB();
    await scrapeData();
    console.log('Scraping complete.');
    process.exit(0);
}

run();
