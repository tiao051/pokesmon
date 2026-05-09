import {computed, type Ref} from "vue";

export function usePagination(
	totalItems: Ref<number>,
	pageSize: Ref<number> | number,
) {
	const limit = computed(() =>
		typeof pageSize === "number" ? pageSize : pageSize.value,
	);
	const totalPages = computed(() =>
		Math.max(1, Math.ceil(totalItems.value / limit.value)),
	);

	const visiblePagesFor = (currentPage: number, delta = 1) => {
		const total = totalPages.value;
		const range: number[] = [];
		for (let i = 1; i <= total; i++) {
			if (
				i === 1 ||
				i === total ||
				(i >= currentPage - delta && i <= currentPage + delta)
			) {
				range.push(i);
			}
		}
		const result: (number | "...")[] = [];
		let last: number | undefined;
		for (const i of range) {
			if (last !== undefined) {
				if (i - last === 2) result.push(last + 1);
				else if (i - last !== 1) result.push("...");
			}
			result.push(i);
			last = i;
		}
		return result;
	};

	return {totalPages, visiblePagesFor};
}
