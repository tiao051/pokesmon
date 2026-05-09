import {useApi} from "../composables/shared/useApi";

export type OrderStatus =
	| "Stocking"
	| "InWarehouse"
	| "Placed"
	| "Shipping"
	| "Delivered"
	| "Cancelled"
	| "Refunded";

export interface OrderItem {
	productId: number;
	slug: string;
	title: string;
	price: number;
	quantity: number;
	image: string;
}

export interface Order {
	id: string;
	userEmail: string;
	items: OrderItem[];
	total: number;
	status: OrderStatus;
	isPreorder: boolean;
	depositAmount?: number | null;
	depositPaid: boolean;
	createdAt: string;
	updatedAt: string;
}

export type OrderListView = "active" | "completed" | "all";

export interface OrderListParams {
	isPreorder?: boolean;
	view?: OrderListView;
}

export interface CreateOrderItem {
	productId: number;
	quantity: number;
}

export interface CreateOrderPayload {
	items: CreateOrderItem[];
	isPreorder: boolean;
}

export const orderService = {
	async list(params: OrderListParams = {}): Promise<Order[]> {
		const {data} = await useApi().get<{items: Order[]}>("/orders", {params});
		return data.items ?? [];
	},

	async getById(id: string): Promise<Order> {
		const {data} = await useApi().get<Order>(`/orders/${id}`);
		return data;
	},

	async create(payload: CreateOrderPayload): Promise<Order> {
		const {data} = await useApi().post<Order>("/orders", payload);
		return data;
	},

	async updateStatus(id: string, status: OrderStatus): Promise<Order> {
		const {data} = await useApi().patch<Order>(`/orders/${id}/status`, {status});
		return data;
	},
};

export const PREORDER_FLOW: OrderStatus[] = [
	"Stocking",
	"InWarehouse",
	"Placed",
	"Shipping",
	"Delivered",
];

export const NORMAL_FLOW: OrderStatus[] = ["Placed", "Shipping", "Delivered"];

export const STATUS_LABELS: Record<OrderStatus, string> = {
	Stocking: "Stocking",
	InWarehouse: "In Warehouse",
	Placed: "Placed",
	Shipping: "Shipping",
	Delivered: "Delivered",
	Cancelled: "Cancelled",
	Refunded: "Refunded",
};

export function flowFor(isPreorder: boolean): OrderStatus[] {
	return isPreorder ? PREORDER_FLOW : NORMAL_FLOW;
}

export function isTerminal(status: OrderStatus): boolean {
	return status === "Delivered" || status === "Cancelled" || status === "Refunded";
}
