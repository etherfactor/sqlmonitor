class Implementation {

  private index: number;

  constructor() {
    this.index = 0;
  }

  getPath(): string {
    const useIndex = this.index++;
    return `e${useIndex}`;
  }
}

export const ɵPrefixGenerator = {
  Implementation,
};
