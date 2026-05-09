import {ref} from "vue";

export function useAsyncSubmit<T>(action: () => Promise<T>) {
	const submitting = ref(false);
	const error = ref<string | null>(null);

	const submit = async (): Promise<T | null> => {
		if (submitting.value) return null;
		error.value = null;
		submitting.value = true;
		try {
			return await action();
		} catch (e) {
			error.value = e instanceof Error ? e.message : "Something went wrong";
			return null;
		} finally {
			submitting.value = false;
		}
	};

	return {submitting, error, submit};
}
