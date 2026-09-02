import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminStatistics, StatisticsPeriod } from '../../models/admin-statistics.model';
import { AdminStatisticsService } from '../../services/admin-statistics.service';

@Component({
    selector: 'app-admin-statistics',
    standalone: true,
    imports: [CommonModule, FormsModule, RouterLink],
    templateUrl: './admin-statistics.html',
    styleUrl: './admin-statistics.css',
})
export class AdminStatisticsPage implements OnInit {
    private readonly statisticsService = inject(AdminStatisticsService);
    private readonly cdr = inject(ChangeDetectorRef);
    period: StatisticsPeriod = 'day';
    selectedDate = '';
    selectedMonth = '';
    selectedYear = new Date().getFullYear();

    statistics: AdminStatistics | null = null;
    loading = false;
    exportingPdf = false;
    exportingWord = false;
    errorMessage = '';

    ngOnInit(): void {
        const now = new Date();

        this.selectedDate = this.toDateInput(now);
        this.selectedMonth = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`;
        this.selectedYear = now.getFullYear();
    }

    loadStatistics(): void {
        if (this.loading) {
            return;
        }

        let date: string | undefined;
        let month: number | undefined;
        let year: number | undefined;

        if (this.period === 'day') {
            if (!this.selectedDate) {
                this.errorMessage = 'Vui lòng chọn ngày.';
                return;
            }

            date = this.selectedDate;
        }

        if (this.period === 'month') {
            if (!this.selectedMonth) {
                this.errorMessage = 'Vui lòng chọn tháng.';
                return;
            }

            const [selectedYear, selectedMonth] = this.selectedMonth
                .split('-')
                .map(Number);

            year = selectedYear;
            month = selectedMonth;
        }

        if (this.period === 'year') {
            year = Number(this.selectedYear);

            if (!year || year < 1 || year > 9999) {
                this.errorMessage = 'Năm không hợp lệ.';
                return;
            }
        }

        this.loading = true;
        this.errorMessage = '';
        this.statistics = null;

        this.cdr.detectChanges();

        this.statisticsService
            .getStatistics(this.period, date, month, year)
            .pipe(
                finalize(() => {
                    this.loading = false;
                    this.cdr.detectChanges();
                }),
            )
            .subscribe({
                next: response => {
                    this.statistics = response;
                },

                error: error => {
                    console.error('Admin statistics error:', error);

                    if (error?.name === 'TimeoutError') {
                        this.errorMessage =
                            'Tải báo cáo quá lâu. Vui lòng thử lại.';
                        return;
                    }

                    this.errorMessage =
                        error?.error?.message ??
                        'Không thể tải dữ liệu thống kê.';
                },
            });
    }
    onPeriodChange(): void {
        this.statistics = null;
    }

    async exportPdf(): Promise<void> {
        if (!this.statistics || this.exportingPdf) {
            return;
        }

        this.exportingPdf = true;

        try {
            const pdfMakeModule = await import('pdfmake/build/pdfmake');
            const pdfFontsModule = await import('pdfmake/build/vfs_fonts');

            const pdfMake: any = (pdfMakeModule as any).default ?? pdfMakeModule;
            const pdfFonts: any = (pdfFontsModule as any).default ?? pdfFontsModule;

            pdfMake.vfs = pdfFonts.pdfMake?.vfs ?? pdfFonts.vfs ?? pdfFonts;

            const salesBody: any[] = [
                [
                    { text: 'Mã đơn', bold: true },
                    { text: 'Khách hàng', bold: true },
                    { text: 'Ngày bán', bold: true },
                    { text: 'Thanh toán', bold: true },
                    { text: 'Tổng tiền', bold: true },
                ],
            ];

            for (const order of this.statistics.salesOrders) {
                salesBody.push([
                    this.shortId(order.id),
                    order.customerName,
                    this.formatDateTime(order.createdAt),
                    order.paymentMethod.toUpperCase(),
                    this.formatMoney(order.totalAmount),
                ]);
            }

            const receiptBody: any[] = [
                [
                    { text: 'Mã phiếu', bold: true },
                    { text: 'Nhà cung cấp', bold: true },
                    { text: 'Ngày nhập', bold: true },
                    { text: 'Tổng chi phí', bold: true },
                ],
            ];

            for (const receipt of this.statistics.inventoryReceipts) {
                receiptBody.push([
                    this.shortId(receipt.id),
                    receipt.supplierName,
                    this.formatDateTime(receipt.receiptDate),
                    this.formatMoney(receipt.totalCost),
                ]);
            }

            const content: any[] = [
                {
                    text: 'BLUE CROWN',
                    fontSize: 12,
                    bold: true,
                    color: '#2563eb',
                    margin: [0, 0, 0, 6],
                },
                {
                    text: 'BÁO CÁO THỐNG KÊ BÁN HÀNG VÀ NHẬP KHO',
                    fontSize: 18,
                    bold: true,
                    color: '#153b8d',
                    margin: [0, 0, 0, 6],
                },
                {
                    text: this.getPeriodTitle(),
                    fontSize: 11,
                    color: '#64748b',
                    margin: [0, 0, 0, 18],
                },
                {
                    table: {
                        widths: ['*', '*'],
                        body: [
                            ['Đơn bán hoàn tất', this.statistics.salesOrderCount.toString()],
                            ['Doanh thu bán hàng', this.formatMoney(this.statistics.salesRevenue)],
                            ['Phiếu nhập đã duyệt', this.statistics.inventoryReceiptCount.toString()],
                            ['Chi phí nhập hàng', this.formatMoney(this.statistics.inventoryCost)],
                            ['Chênh lệch thu - chi', this.formatMoney(this.statistics.balance)],
                        ],
                    },
                    margin: [0, 0, 0, 20],
                },
                {
                    text: 'HÓA ĐƠN / ĐƠN BÁN ĐÃ HOÀN TẤT',
                    fontSize: 13,
                    bold: true,
                    color: '#153b8d',
                    margin: [0, 0, 0, 8],
                },
            ];

            if (this.statistics.salesOrders.length > 0) {
                content.push({
                    table: {
                        headerRows: 1,
                        widths: ['auto', '*', 'auto', 'auto', 'auto'],
                        body: salesBody,
                    },
                    layout: 'lightHorizontalLines',
                    fontSize: 9,
                    margin: [0, 0, 0, 20],
                });
            } else {
                content.push({
                    text: 'Không có đơn bán hoàn tất trong kỳ.',
                    italics: true,
                    color: '#64748b',
                    margin: [0, 0, 0, 20],
                });
            }

            content.push({
                text: 'PHIẾU NHẬP KHO ĐÃ DUYỆT',
                fontSize: 13,
                bold: true,
                color: '#153b8d',
                margin: [0, 0, 0, 8],
            });

            if (this.statistics.inventoryReceipts.length > 0) {
                content.push({
                    table: {
                        headerRows: 1,
                        widths: ['auto', '*', 'auto', 'auto'],
                        body: receiptBody,
                    },
                    layout: 'lightHorizontalLines',
                    fontSize: 9,
                });
            } else {
                content.push({
                    text: 'Không có phiếu nhập đã duyệt trong kỳ.',
                    italics: true,
                    color: '#64748b',
                });
            }

            content.push({
                text: 'Chênh lệch thu - chi chỉ phản ánh doanh thu bán hàng trừ chi phí nhập hàng, không phải lợi nhuận kế toán.',
                fontSize: 8,
                italics: true,
                color: '#64748b',
                margin: [0, 20, 0, 0],
            });

            const definition: any = {
                pageSize: 'A4',
                pageOrientation: 'landscape',
                pageMargins: [35, 35, 35, 35],
                defaultStyle: {
                    font: 'Roboto',
                    fontSize: 10,
                },
                content,
            };

            pdfMake.createPdf(definition).download(
                `blue-crown-thong-ke-${this.getFileSuffix()}.pdf`,
            );
        } catch (error) {
            console.error(error);
            this.errorMessage = 'Không thể xuất file PDF.';
        } finally {
            this.exportingPdf = false;
        }
    }

    async exportWord(): Promise<void> {
        if (!this.statistics || this.exportingWord) {
            return;
        }

        this.exportingWord = true;

        try {
            const docx = await import('docx');

            const {
                AlignmentType,
                Document,
                HeadingLevel,
                Packer,
                Paragraph,
                Table,
                TableCell,
                TableRow,
                TextRun,
                WidthType,
            } = docx;

            const cell = (text: string, bold = false): InstanceType<typeof TableCell> =>
                new TableCell({
                    children: [
                        new Paragraph({
                            children: [
                                new TextRun({
                                    text,
                                    bold,
                                }),
                            ],
                        }),
                    ],
                });

            const summaryTable = new Table({
                width: {
                    size: 100,
                    type: WidthType.PERCENTAGE,
                },
                rows: [
                    new TableRow({
                        children: [
                            cell('Chỉ tiêu', true),
                            cell('Giá trị', true),
                        ],
                    }),
                    new TableRow({
                        children: [
                            cell('Đơn bán hoàn tất'),
                            cell(this.statistics.salesOrderCount.toString()),
                        ],
                    }),
                    new TableRow({
                        children: [
                            cell('Doanh thu bán hàng'),
                            cell(this.formatMoney(this.statistics.salesRevenue)),
                        ],
                    }),
                    new TableRow({
                        children: [
                            cell('Phiếu nhập đã duyệt'),
                            cell(this.statistics.inventoryReceiptCount.toString()),
                        ],
                    }),
                    new TableRow({
                        children: [
                            cell('Chi phí nhập hàng'),
                            cell(this.formatMoney(this.statistics.inventoryCost)),
                        ],
                    }),
                    new TableRow({
                        children: [
                            cell('Chênh lệch thu - chi'),
                            cell(this.formatMoney(this.statistics.balance)),
                        ],
                    }),
                ],
            });

            const salesRows = [
                new TableRow({
                    children: [
                        cell('Mã đơn', true),
                        cell('Khách hàng', true),
                        cell('Ngày bán', true),
                        cell('Thanh toán', true),
                        cell('Tổng tiền', true),
                    ],
                }),
                ...this.statistics.salesOrders.map(order =>
                    new TableRow({
                        children: [
                            cell(this.shortId(order.id)),
                            cell(order.customerName),
                            cell(this.formatDateTime(order.createdAt)),
                            cell(order.paymentMethod.toUpperCase()),
                            cell(this.formatMoney(order.totalAmount)),
                        ],
                    }),
                ),
            ];

            const receiptRows = [
                new TableRow({
                    children: [
                        cell('Mã phiếu', true),
                        cell('Nhà cung cấp', true),
                        cell('Ngày nhập', true),
                        cell('Tổng chi phí', true),
                    ],
                }),
                ...this.statistics.inventoryReceipts.map(receipt =>
                    new TableRow({
                        children: [
                            cell(this.shortId(receipt.id)),
                            cell(receipt.supplierName),
                            cell(this.formatDateTime(receipt.receiptDate)),
                            cell(this.formatMoney(receipt.totalCost)),
                        ],
                    }),
                ),
            ];

            const children: any[] = [
                new Paragraph({
                    alignment: AlignmentType.CENTER,
                    children: [
                        new TextRun({
                            text: 'BLUE CROWN',
                            bold: true,
                            size: 24,
                        }),
                    ],
                }),
                new Paragraph({
                    alignment: AlignmentType.CENTER,
                    heading: HeadingLevel.TITLE,
                    text: 'BÁO CÁO THỐNG KÊ BÁN HÀNG VÀ NHẬP KHO',
                }),
                new Paragraph({
                    alignment: AlignmentType.CENTER,
                    text: this.getPeriodTitle(),
                    spacing: {
                        after: 300,
                    },
                }),
                new Paragraph({
                    heading: HeadingLevel.HEADING_2,
                    text: '1. Tổng quan',
                }),
                summaryTable,
                new Paragraph({
                    heading: HeadingLevel.HEADING_2,
                    text: '2. Hóa đơn / đơn bán đã hoàn tất',
                    spacing: {
                        before: 300,
                    },
                }),
            ];

            if (this.statistics.salesOrders.length > 0) {
                children.push(
                    new Table({
                        width: {
                            size: 100,
                            type: WidthType.PERCENTAGE,
                        },
                        rows: salesRows,
                    }),
                );
            } else {
                children.push(
                    new Paragraph({
                        text: 'Không có đơn bán hoàn tất trong kỳ.',
                    }),
                );
            }

            children.push(
                new Paragraph({
                    heading: HeadingLevel.HEADING_2,
                    text: '3. Phiếu nhập kho đã duyệt',
                    spacing: {
                        before: 300,
                    },
                }),
            );

            if (this.statistics.inventoryReceipts.length > 0) {
                children.push(
                    new Table({
                        width: {
                            size: 100,
                            type: WidthType.PERCENTAGE,
                        },
                        rows: receiptRows,
                    }),
                );
            } else {
                children.push(
                    new Paragraph({
                        text: 'Không có phiếu nhập đã duyệt trong kỳ.',
                    }),
                );
            }

            children.push(
                new Paragraph({
                    spacing: {
                        before: 300,
                    },
                    children: [
                        new TextRun({
                            text: 'Lưu ý: Chênh lệch thu - chi chỉ phản ánh doanh thu bán hàng trừ chi phí nhập hàng, không phải lợi nhuận kế toán.',
                            italics: true,
                        }),
                    ],
                }),
            );

            const document = new Document({
                sections: [
                    {
                        children,
                    },
                ],
            });

            const blob = await Packer.toBlob(document);

            this.downloadBlob(
                blob,
                `blue-crown-thong-ke-${this.getFileSuffix()}.docx`,
            );
        } catch (error) {
            console.error(error);
            this.errorMessage = 'Không thể xuất file Word.';
        } finally {
            this.exportingWord = false;
        }
    }

    formatMoney(value: number): string {
        return new Intl.NumberFormat('vi-VN').format(value) + ' đ';
    }

    shortId(id: string): string {
        return id.substring(0, 8).toUpperCase();
    }

    getPeriodTitle(): string {
        if (this.period === 'day') {
            return `Ngày ${this.formatInputDate(this.selectedDate)}`;
        }

        if (this.period === 'month') {
            const [year, month] = this.selectedMonth.split('-');
            return `Tháng ${month}/${year}`;
        }

        return `Năm ${this.selectedYear}`;
    }

    private formatDateTime(value: string | null): string {
        if (!value) {
            return '';
        }

        return new Intl.DateTimeFormat('vi-VN', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
        }).format(new Date(value));
    }

    private getFileSuffix(): string {
        if (this.period === 'day') {
            return this.selectedDate;
        }

        if (this.period === 'month') {
            return this.selectedMonth;
        }

        return String(this.selectedYear);
    }

    private formatInputDate(value: string): string {
        if (!value) {
            return '';
        }

        const [year, month, day] = value.split('-');
        return `${day}/${month}/${year}`;
    }

    private toDateInput(date: Date): string {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');

        return `${year}-${month}-${day}`;
    }

    private downloadBlob(blob: Blob, fileName: string): void {
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');

        link.href = url;
        link.download = fileName;
        link.click();

        URL.revokeObjectURL(url);
    }
}